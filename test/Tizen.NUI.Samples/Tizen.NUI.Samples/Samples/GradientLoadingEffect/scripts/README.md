# Scripts

## record-demo

Records the NUI GradientLoadingEffect sample as an mp4.
The script opens the app at `700x700`, records one radial pass and one linear pass,
then closes the app.

Requirements:

- X11 display
- `ffmpeg`

Usage:

```sh
cd ~/Shared/TEST/NUI/TizenNUISamples/TizenNUISamples
scripts/record-demo
```

Options:

```sh
scripts/record-demo --output ~/Videos/nui-demo.mp4
scripts/record-demo --window-size 800x800
```
