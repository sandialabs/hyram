
#!/bin/bash

# Creates Windows .ico file container containing scaled images from single logo .png of logo

# Create various sizes
for size in 16 32 48 128 256; do
    C:/Program\ Files/Inkscape/bin/inkscapecom.com --export-type=png --export-filename="$size.png" -w ${size} -h ${size} -b white --export-png-use-dithering=true icon512.png
done

# mash them into single .ico
C:/Program\ Files/ImageMagick-7.1.1-Q16-HDRI/convert.exe 16.png 32.png 48.png 128.png 256.png -colors 256 icon.ico