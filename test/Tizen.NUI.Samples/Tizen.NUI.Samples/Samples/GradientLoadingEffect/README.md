# Gradient Loading Effect Sample (Tizen.NUI)

## Overview

This is the C# / Tizen.NUI version of the Gradient Loading Effect example.

The sample demonstrates:
- **Color Gradient**: A smooth multi-color gradient visual
- **Alpha Mask Gradient**: Radial or linear gradient masks that reveal content
- **Smooth Transitions**: Animated transitions with gradient reveals
- **Interactive Controls**: Keyboard input to switch between mask types

## Features

- **Radial Mask Mode**: Circular gradient mask with smooth reveal
- **Linear Mask Mode**: Linear gradient mask with smooth reveal
- **Content Switching**: Cycles through images and text card
- **Interactive Controls**: 
  - Press `1` for radial mode
  - Press `2` for linear mode
  - Touch/Click to trigger transitions

## Usage

The sample is integrated into the Tizen.NUI.Samples application.

### Running the Sample

```bash
cd ~/Shared/NUI/TizenFX/test/Tizen.NUI.Samples
# Build and run the samples
dotnet build
# Then select "Gradient Loading Effect" from the samples menu
```

### Recording a Demo

```bash
Samples/GradientLoadingEffect/scripts/record-demo
```

This will record a demo video with both radial and linear transitions.

## Implementation

The sample implements the `IExample` interface, making it compatible with the Tizen.NUI.Samples framework:

- **GradientLoadingEffectSample.cs** - Main implementation
- **Activate()** - Called when sample is displayed
- **Deactivate()** - Called when sample is hidden or closed

## Files

```
GradientLoadingEffect/
├── README.md                    This file
└── scripts/
    ├── record-demo             Recording script
    └── README.md               Script documentation
```

## Differences from C++ Version

| Aspect | C++ (DALi) | C# (Tizen.NUI) |
|--------|-----------|----------------|
| Gradient API | `Toolkit::GradientVisual` | `Visual.Type.Gradient` |
| Mask Effect | `Toolkit::MaskEffect` | `RenderEffect.CreateMaskEffect()` |
| Animation | `Dali::Animation` | `Animation` |
| Image Path | DEMO_IMAGE_DIR | CommonResource paths |
| Namespace | Dali | Tizen.NUI |

## Performance Notes

- Tizen.NUI samples run on the same DALi engine as native C++ apps
- Performance is comparable to the C++ version
- GPU-accelerated rendering via RenderEffect

## License

Copyright (c) 2026 Samsung Electronics Co., Ltd.
Licensed under the Apache License, Version 2.0
