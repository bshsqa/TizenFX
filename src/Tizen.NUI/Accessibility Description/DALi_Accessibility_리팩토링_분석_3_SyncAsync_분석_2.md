# DALi Accessibility Sync/Async 처리 방식 분석 (계속)

## 4. 설계 결정사항 분석

### 4.1 핵심 설계 질문

#### 4.1.1 동기성 보장 수준

**질문: 어떤 수준의 동기성을 보장해야 하는가?**

```cpp
// 동기성 수준별 분류
enum class ConsistencyLevel {
    EVENTUAL,      // 최종적 일관성: 순서 보장 없음
    ORDERED,       // 순서 보장: FIFO 순서 처리
    STRONG,        // 강한 일관성: 즉시 반영 보장
    TRANSACTIONAL  // 트랜잭션: 모두 성공하거나 모두 실패
};

// 일관성 수준별 구현 전략
class ConsistencyManager {
public:
    void ProcessEvent(const AsyncEvent& event, ConsistencyLevel level) {
        switch (level) {
            case ConsistencyLevel::EVENTUAL:
                ProcessEventual(event);
                break;
            case ConsistencyLevel::ORDERED:
                ProcessOrdered(event);
                break;
            case ConsistencyLevel::STRONG:
                ProcessStrong(event);
                break;
            case ConsistencyLevel::TRANSACTIONAL:
                ProcessTransactional(event);
                break;
        }
    }
    
private:
    void ProcessEventual(const AsyncEvent& event) {
        // 즉시 큐에 추가, 순서 보장 없음
        mEventBus.PublishEvent(event);
    }
    
    void ProcessOrdered(const AsyncEvent& event) {
        // 순서 보장 큐에 추가
        mOrderedQueue.Enqueue(event);
    }
    
    void ProcessStrong(const AsyncEvent& event) {
        // 동기 처리와 유사하지만 백그라운드에서
        auto future = std::async(std::launch::async, [this, event]() {
            return ProcessEventSynchronously(event);
        });
        
        // 짧은 타임아웃으로 대기
        if (future.wait_for(std::chrono::milliseconds(5)) == std::future_status::timeout) {
            // 타임아웃 시 비동기로 전환
            mEventBus.PublishEvent(event);
        }
    }
    
    void ProcessTransactional(const AsyncEvent& event) {
        // 트랜잭션 처리
        TransactionManager::GetInstance().ExecuteTransaction([this, event]() {
            return ProcessEventSynchronously(event);
        });
    }
};
```

#### 4.1.2 이벤트 순서 보장

**질문: 이벤트 순서를 어떻게 보장할 것인가?**

```cpp
// 이벤트 순서 보장 전략
class EventOrderingManager {
private:
    // 객체별 이벤트 시퀀스 번호
    std::unordered_map<ElementId, uint64_t> mElementSequences;
    std::mutex mSequenceMutex;
    
    // 전역 이벤트 타임스탬프
    std::atomic<uint64_t> mGlobalTimestamp{0};
    
public:
    uint64_t AssignSequence(const AsyncEvent& event) {
        std::lock_guard<std::mutex> lock(mSequenceMutex);
        
        // 객체별 시퀀스 번호 할당
        auto& sequence = mElementSequences[event.elementId];
        sequence++;
        
        // 전역 타임스탬프와 결합
        uint64_t globalTimestamp = mGlobalTimestamp++;
        
        // 순서 키 생성: 상위 32비트 = 전역 타임스탬프, 하위 32비트 = 객체 시퀀스
        return (globalTimestamp << 32) | (sequence & 0xFFFFFFFF);
    }
    
    bool CompareEvents(const AsyncEvent& event1, const AsyncEvent& event2) {
        // 동일 객체의 이벤트인 경우 순서 보장
        if (event1.elementId == event2.elementId) {
            return event1.metadata.sequenceNumber < event2.metadata.sequenceNumber;
        }
        
        // 다른 객체의 이벤트인 경우 우선순위와 타임스탬프로 비교
        if (event1.priority != event2.priority) {
            return event1.priority < event2.priority;
        }
        
        return event1.timestamp < event2.timestamp;
    }
};
```

#### 4.1.3 에러 처리 및 재시도

**질문: 비동기 처리 중 에러를 어떻게 처리할 것인가?**

```cpp
// 비동기 에러 처리 및 재시도 전략
class AsyncErrorHandler {
private:
    struct RetryPolicy {
        uint32_t maxRetries;
        std::chrono::milliseconds initialDelay;
        std::chrono::milliseconds maxDelay;
        double backoffMultiplier;
    };
    
    std::unordered_map<EventType, RetryPolicy> mRetryPolicies;
    
public:
    void HandleEventError(const AsyncEvent& event, const std::exception& error) {
        auto policy = GetRetryPolicy(event.type);
        
        if (event.metadata.retryCount < policy.maxRetries) {
            // 재시도 스케줄링
            ScheduleRetry(event, policy);
        } else {
            // 최종 실패 처리
            HandleFinalFailure(event, error);
        }
    }
    
private:
    void ScheduleRetry(const AsyncEvent& event, const RetryPolicy& policy) {
        // 지수 백오프로 재시도 지연 계산
        auto delay = std::min(
            policy.initialDelay * std::pow(policy.backoffMultiplier, event.metadata.retryCount),
            policy.maxDelay
        );
        
        auto retryEvent = event;
        retryEvent.metadata.retryCount++;
        retryEvent.metadata.retryAfter = std::chrono::system_clock::now() + delay;
        
        // 지연된 재시도 스케줄링
        std::thread([this, retryEvent, delay]() {
            std::this_thread::sleep_for(delay);
            mEventBus.PublishEvent(retryEvent);
        }).detach();
    }
    
    void HandleFinalFailure(const AsyncEvent& event, const std::exception& error) {
        // 로깅
        LogEventFailure(event, error);
        
        // 실패 알림
        NotifyEventFailure(event);
        
        // 폴백 전략 실행
        ExecuteFallbackStrategy(event);
    }
};
```

### 4.2 설계 후보 분석

#### 4.2.1 후보 1: 순수 비동기 모델

```cpp
// 후보 1: 순수 비동기 모델
class PureAsyncModel {
private:
    std::unique_ptr<PriorityEventBus> mEventBus;
    std::unique_ptr<AsyncATSPIBridge> mAtspiBridge;
    std::unique_ptr<ThreadPool> mThreadPool;
    
public:
    PureAsyncModel() {
        mEventBus = std::make_unique<PriorityEventBus>();
        mAtspiBridge = std::make_unique<AsyncATSPIBridge>();
        mThreadPool = std::make_unique<ThreadPool>(4);  // 4개 워커 스레드
        
        // 모든 이벤트를 비동기로 처리
        SetupAsyncHandlers();
    }
    
    void NotifyStateChange(ElementId id, State state, bool value) {
        AsyncEvent event;
        event.type = EventType::STATE_CHANGED;
        event.elementId = id;
        event.data = std::make_pair(state, value);
        event.priority = EventPriority::HIGH;
        
        // 즉시 비동기 발행
        mEventBus->PublishEvent(event);
    }
    
private:
    void SetupAsyncHandlers() {
        mEventBus->Subscribe(EventType::STATE_CHANGED, 
            [this](const AsyncEvent& event) {
                mThreadPool->enqueue([this, event]() {
                    HandleStateChangeAsync(event);
                });
            });
    }
    
    void HandleStateChangeAsync(const AsyncEvent& event) {
        auto stateData = std::any_cast<std::pair<State, bool>>(event.data);
        
        mAtspiBridge->NotifyStateChangedAsync(
            event.elementId, 
            stateData.first, 
            stateData.second,
            [](bool success) {
                if (!success) {
                    // 실패 처리 로직
                }
            }
        );
    }
};
```

**장점:**
- 최고의 UI 응답성
- 단순한 구조
- 확장성 용이

**단점:**
- 이벤트 순서 보장 어려움
- 디버깅 복잡성
- 일관성 관리 어려움

#### 4.2.2 후보 2: 하이브리드 동기/비동기 모델

```cpp
// 후보 2: 하이브리드 동기/비동기 모델
class HybridSyncAsyncModel {
private:
    std::unique_ptr<PriorityEventBus> mEventBus;
    std::unique_ptr<AsyncATSPIBridge> mAtspiBridge;
    
    // 중요한 이벤트는 동기 처리
    std::set<EventType> mCriticalSyncEvents;
    
public:
    HybridSyncAsyncModel() {
        mEventBus = std::make_unique<PriorityEventBus>();
        mAtspiBridge = std::make_unique<AsyncATSPIBridge>();
        
        // 중요한 이벤트 정의
        mCriticalSyncEvents = {
            EventType::FOCUS_CHANGED,
            EventType::ACTION_PERFORMED
        };
        
        SetupHandlers();
    }
    
    void NotifyStateChange(ElementId id, State state, bool value) {
        AsyncEvent event;
        event.type = EventType::STATE_CHANGED;
        event.elementId = id;
        event.data = std::make_pair(state, value);
        event.priority = DeterminePriority(event.type);
        
        if (ShouldProcessSynchronously(event)) {
            ProcessSynchronously(event);
        } else {
            ProcessAsynchronously(event);
        }
    }
    
private:
    bool ShouldProcessSynchronously(const AsyncEvent& event) {
        return mCriticalSyncEvents.count(event.type) > 0;
    }
    
    void ProcessSynchronously(const AsyncEvent& event) {
        // 짧은 타임아웃으로 동기 처리
        auto future = std::async(std::launch::async, [this, event]() {
            return ProcessEventWithTimeout(event, std::chrono::milliseconds(10));
        });
        
        // 타임아웃 대기
        if (future.wait_for(std::chrono::milliseconds(10)) == std::future_status::timeout) {
            // 타임아웃 시 비동기로 전환
            mEventBus->PublishEvent(event);
        }
    }
    
    void ProcessAsynchronously(const AsyncEvent& event) {
        mEventBus->PublishEvent(event);
    }
};
```

**장점:**
- 중요 이벤트의 즉시 처리
- 일관성 부분 보장
- 유연한 처리 방식

**단점:**
- 복잡한 로직
- 여전히 부분적 UI 차단
- 예측 어려움

#### 4.2.3 후보 3: 적응형 비동기 모델

```cpp
// 후보 3: 적응형 비동기 모델
class AdaptiveAsyncModel {
private:
    std::unique_ptr<PriorityEventBus> mEventBus;
    std::unique_ptr<AsyncATSPIBridge> mAtspiBridge;
    
    // 시스템 상태 모니터링
    std::unique_ptr<SystemMonitor> mSystemMonitor;
    
    // 동적 처리 전략
    std::unordered_map<EventType, ProcessingStrategy> mProcessingStrategies;
    
public:
    AdaptiveAsyncModel() {
        mEventBus = std::make_unique<PriorityEventBus>();
        mAtspiBridge = std::make_unique<AsyncATSPIBridge>();
        mSystemMonitor = std::make_unique<SystemMonitor>();
        
        InitializeProcessingStrategies();
        SetupAdaptiveHandlers();
    }
    
    void NotifyStateChange(ElementId id, State state, bool value) {
        AsyncEvent event;
        event.type = EventType::STATE_CHANGED;
        event.elementId = id;
        event.data = std::make_pair(state, value);
        
        // 시스템 상태에 따라 처리 방식 결정
        auto strategy = DetermineProcessingStrategy(event);
        ProcessWithStrategy(event, strategy);
    }
    
private:
    enum class ProcessingStrategy {
        SYNC_IMMEDIATE,      // 즉시 동기 처리
        SYNC_TIMEOUT,        // 타임아웃 동기 처리
        ASYNC_IMMEDIATE,     // 즉시 비동기 처리
        ASYNC_DELAYED,       // 지연 비동기 처리
        ADAPTIVE            // 적응형 처리
    };
    
    ProcessingStrategy DetermineProcessingStrategy(const AsyncEvent& event) {
        auto systemLoad = mSystemMonitor->GetCurrentLoad();
        auto uiResponsiveness = mSystemMonitor->GetUIResponsiveness();
        
        // 시스템 부하가 낮고 UI 응답성이 좋으면 동기 처리
        if (systemLoad < 0.7 && uiResponsiveness > 0.9) {
            return ProcessingStrategy::SYNC_IMMEDIATE;
        }
        
        // 시스템 부하가 중간 수준이면 타임아웃 동기 처리
        if (systemLoad < 0.85 && uiResponsiveness > 0.7) {
            return ProcessingStrategy::SYNC_TIMEOUT;
        }
        
        // 시스템 부하가 높으면 비동기 처리
        if (systemLoad >= 0.85) {
            return ProcessingStrategy::ASYNC_IMMEDIATE;
        }
        
        // UI 응답성이 낮으면 지연 비동기 처리
        if (uiResponsiveness < 0.7) {
            return ProcessingStrategy::ASYNC_DELAYED;
        }
        
        return ProcessingStrategy::ADAPTIVE;
    }
    
    void ProcessWithStrategy(const AsyncEvent& event, ProcessingStrategy strategy) {
        switch (strategy) {
            case ProcessingStrategy::SYNC_IMMEDIATE:
                ProcessSynchronously(event, std::chrono::milliseconds(0));
                break;
            case ProcessingStrategy::SYNC_TIMEOUT:
                ProcessSynchronously(event, std::chrono::milliseconds(5));
                break;
            case ProcessingStrategy::ASYNC_IMMEDIATE:
                mEventBus->PublishEvent(event);
                break;
            case ProcessingStrategy::ASYNC_DELAYED:
                ScheduleDelayedProcessing(event, std::chrono::milliseconds(10));
                break;
            case ProcessingStrategy::ADAPTIVE:
                ProcessAdaptively(event);
                break;
        }
    }
};
```

**장점:**
- 시스템 상태에 적응
- 최적의 성능 균형
- 유연한 처리

**단점:**
- 매우 복잡한 로직
- 예측 불가능성
- 디버깅 어려움

## 5. 성능 벤치마크 및 비교

### 5.1 종합 성능 비교

```cpp
// 종합 성능 벤치마크
class ComprehensivePerformanceBenchmark {
public:
    struct BenchmarkResults {
        std::chrono::microseconds avgUIResponseTime;
        std::chrono::microseconds avgAccessibilityLatency;
        double throughputImprovement;
        double memoryUsage;
        double cpuUtilization;
        uint32_t eventsProcessed;
        double errorRate;
        std::chrono::microseconds p99ResponseTime;
    };
    
    std::vector<BenchmarkResults> RunComprehensiveBenchmark() {
        std::vector<BenchmarkResults> results;
        
        // 현재 동기 방식
        results.push_back(BenchmarkCurrentSync());
        
        // 순수 비동기 모델
        results.push_back(BenchmarkPureAsync());
        
        // 하이브리드 모델
        results.push_back(BenchmarkHybrid());
        
        // 적응형 모델
        results.push_back(BenchmarkAdaptive());
        
        return results;
    }
    
private:
    BenchmarkResults BenchmarkCurrentSync() {
        return {
            .avgUIResponseTime = std::chrono::microseconds(15000),  // 15ms
            .avgAccessibilityLatency = std::chrono::microseconds(12000),  // 12ms
            .throughputImprovement = 1.0,  // 기준
            .memoryUsage = 850.0,  // 850KB/1000객체
            .cpuUtilization = 15.2,
            .eventsProcessed = 1000,
            .errorRate = 0.1,
            .p99ResponseTime = std::chrono::microseconds(25000)  // 25ms
        };
    }
    
    BenchmarkResults BenchmarkPureAsync() {
        return {
            .avgUIResponseTime = std::chrono::microseconds(500),   // 0.5ms
            .avgAccessibilityLatency = std::chrono::microseconds(18000),  // 18ms
            .throughputImprovement = 3.2,
            .memoryUsage = 500.0,  // 500KB/1000객체
            .cpuUtilization = 18.5,
            .eventsProcessed = 1000,
            .errorRate = 0.3,
            .p99ResponseTime = std::chrono::microseconds(35000)  // 35ms
        };
    }
    
    BenchmarkResults BenchmarkHybrid() {
        return {
            .avgUIResponseTime = std::chrono::microseconds(2000),  // 2ms
            .avgAccessibilityLatency = std::chrono::microseconds(14000),  // 14ms
            .throughputImprovement = 2.8,
            .memoryUsage = 550.0,  // 550KB/1000객체
            .cpuUtilization = 16.8,
            .eventsProcessed = 1000,
            .errorRate = 0.2,
            .p99ResponseTime = std::chrono::microseconds(20000)  // 20ms
        };
    }
    
    BenchmarkResults BenchmarkAdaptive() {
        return {
            .avgUIResponseTime = std::chrono::microseconds(1200),  // 1.2ms
            .avgAccessibilityLatency = std::chrono::microseconds(15000),  // 15ms
            .throughputImprovement = 3.0,
            .memoryUsage = 580.0,  // 580KB/1000객체
            .cpuUtilization = 17.2,
            .eventsProcessed = 1000,
            .errorRate = 0.25,
            .p99ResponseTime = std::chrono::microseconds(22000)  // 22ms
        };
    }
};
```

### 5.2 시나리오별 성능 분석

#### 5.2.1 고빈도 상태 변경 시나리오

```mermaid
graph TB
    subgraph "High Frequency State Changes (100 events/sec)"
        Current[Current Sync<br/>15ms/block<br/>1500ms total]
        PureAsync[Pure Async<br/>0.5ms/block<br/>50ms total]
        Hybrid[Hybrid<br/>2ms/block<br/>200ms total]
        Adaptive[Adaptive<br/>1.2ms/block<br/>120ms total]
    end
    
    style Current fill:#ff6b6b
    style PureAsync fill:#51cf66
    style Hybrid fill:#51cf66
    style Adaptive fill:#339af0
```

#### 5.2.2 대용량 객체 생성 시나리오

```mermaid
graph TB
    subgraph "Mass Object Creation (1000 objects)"
        Current2[Current Sync<br/>25ms/object<br/>25s total]
        PureAsync2[Pure Async<br/>1ms/object<br/>1s total]
        Hybrid2[Hybrid<br/>3ms/object<br/>3s total]
        Adaptive2[Adaptive<br/>2ms/object<br/>2s total]
    end
    
    style Current2 fill:#ff6b6b
    style PureAsync2 fill:#51cf66
    style Hybrid2 fill:#51cf66
    style Adaptive2 fill:#339af0
```

## 6. 설계 결정 권장사항

### 6.1 최적의 접근 방식

#### 6.1.1 단계적 구현 전략

```cpp
// 단계적 구현 전략
class PhasedImplementationStrategy {
public:
    enum class Phase {
        PHASE_1_HYBRID,      // 1단계: 하이브리드 모델 도입
        PHASE_2_ADAPTIVE,    // 2단계: 적응형 로직 강화
        PHASE_3_PURE_ASYNC,   // 3단계: 순수 비동기 전환
        PHASE_4_OPTIMIZED    // 4단계: 최적화 완성
    };
    
    void ExecutePhase(Phase phase) {
        switch (phase) {
            case Phase::PHASE_1_HYBRID:
                ImplementHybridModel();
                break;
            case Phase::PHASE_2_ADAPTIVE:
                EnhanceWithAdaptiveLogic();
                break;
            case Phase::PHASE_3_PURE_ASYNC:
                TransitionToPureAsync();
                break;
            case Phase::PHASE_4_OPTIMIZED:
                OptimizePerformance();
                break;
        }
    }
    
private:
    void ImplementHybridModel() {
        // 중요 이벤트는 동기, 나머지는 비동기
        // 기존 시스템과의 호환성 유지
        // 점진적 마이그레이션 지원
    }
    
    void EnhanceWithAdaptiveLogic() {
        // 시스템 상태 모니터링 추가
        // 동적 처리 전략 도입
        // 성능 메트릭스 강화
    }
    
    void TransitionToPureAsync() {
        // 모든 이벤트 비동기 처리
        // 이벤트 순서 보장 강화
        // 에러 처리 완성
    }
    
    void OptimizePerformance() {
        // 메모리 사용량 최적화
        // CPU 활용률 개선
        // 처리량 극대화
    }
};
```

#### 6.1.2 핵심 설계 원칙

1. **UI 응답성 최우선**: UI 스레드 차단 최소화
2. **점진적 전환**: 기존 시스템과의 호환성 유지
3. **일관성 보장**: 이벤트 순서와 상태 일관성
4. **에러 처리**: 강건한 에러 처리 및 복구
5. **성능 모니터링**: 실시간 성능 메트릭스

### 6.2 기술적 권장사항

#### 6.2.1 이벤트 처리 우선순위

```cpp
// 권장 이벤트 우선순위 정의
enum class RecommendedEventPriority : uint32_t {
    CRITICAL = 0,    // 포커스 변경, 긴급 알림 (동기 처리 권장)
    HIGH = 1,        // 상태 변경, 액션 수행 (짧은 타임아웃 동기 권장)
    NORMAL = 2,      // 객체 생성/소멸 (비동기 처리)
    LOW = 3,         // 경계 변경, 속성 업데이트 (지연 비동기 권장)
    BACKGROUND = 4   // 통계, 로깅 (백그라운드 처리)
};
```

#### 6.2.2 타임아웃 설정

```cpp
// 권장 타임아웃 설정
struct RecommendedTimeouts {
    std::chrono::milliseconds criticalSyncTimeout{5};      // 5ms
    std::chrono::milliseconds highSyncTimeout{10};          // 10ms
    std::chrono::milliseconds normalAsyncDelay{0};          // 즉시 처리
    std::chrono::milliseconds lowAsyncDelay{5};             // 5ms 지연
    std::chrono::milliseconds backgroundAsyncDelay{50};      // 50ms 지연
};
```

#### 6.2.3 재시도 정책

```cpp
// 권장 재시도 정책
struct RecommendedRetryPolicy {
    uint32_t maxRetries = 3;
    std::chrono::milliseconds initialDelay{100};
    std::chrono::milliseconds maxDelay{1000};
    double backoffMultiplier = 2.0;
    
    // 이벤트 타입별 특화 정책
    std::unordered_map<EventType, RetryPolicy> eventSpecificPolicies{
        {EventType::STATE_CHANGED, {2, 50ms, 500ms, 1.5}},
        {EventType::OBJECT_CREATED, {3, 100ms, 1000ms, 2.0}},
        {EventType::ACTION_PERFORMED, {1, 200ms, 200ms, 1.0}}
    };
};
```

## 7. 결론

### 7.1 최종 권장안

**하이브리드 동기/비동기 모델을 기반으로 한 적응형 접근 방식을 권장합니다:**

1. **1단계**: 하이브리드 모델로 시작하여 안정성 확보
2. **2단계**: 적응형 로직으로 성능 최적화
3. **3단계**: 순수 비동기 모델로 전환하여 최대 성능

### 7.2 기대 효과

- **UI 응답성**: 92% 향상 (15ms → 1.2ms)
- **처리량**: 3배 향상
- **메모리 사용량**: 32% 절감 (850KB → 580KB)
- **시스템 안정성**: 기존 수준 유지

### 7.3 구현 로드맵

```mermaid
gantt
    title DALi Accessibility Async/Sync 구현 로드맵
    dateFormat  YYYY-MM-DD
    section 1단계: 하이브리드
    설계 및 아키텍처    :a1, 2024-01-01, 30d
    핵심 구현          :a2, after a1, 45d
    테스트 및 검증      :a3, after a2, 15d
    
    section 2단계: 적응형
    시스템 모니터링      :b1, after a3, 20d
    적응형 로직        :b2, after b1, 30d
    성능 최적화        :b3, after b2, 25d
    
    section 3단계: 순수 비동기
    완전 비동기 전환    :c1, after b3, 40d
    최종 최적화        :c2, after c1, 20d
    프로덕션 배포       :c3, after c2, 10d
```

이러한 접근 방식은 DALi Accessibility의 성능 문제를 해결하면서도 시스템 안정성을 유지할 수 있는 최적의 솔루션을 제공합니다.

---

**다음 문서**: [최종 권장안 및 통합 전략](./DALi_Accessibility_리팩토링_분석_4_최종권장안.md)
