# Stroblhofwarte.Photometrie

Stroblhofwarte.Photometrie is a tool for star photometrie on CCD/CMOS images:
- It get's the data of the variable star on your image via AAVSO VSX api
- It can solve your image using the Astrometry.net api (API key required, it's for free!)
- It can annotate the image with the reference stars for your measurement
- Your equipment can be calibrated using standard star fields. M67 is build in, other fields can be added via .txt file
- Your measurement can be transformed to match the standard photometry color system (for now UBV[RI])
- Single measurements or groups of measurements can be reportet to the AAVSO
- All data (calibration, CMOS properties, used aperture etc.) are stored to the equipment database
- All equipment data are read out of the .fits file and matched with the equipment database
- Only .fits files are supported. Many parts of the code are from the N.I.N.A. project, only .fits-Files from N.I.N.A. are tested yet

![Stroblhofwarte.Photometrie](https://github.com/stroblhofwarte/Stroblhofwarte.Photometrie/blob/master/Stroblhofwarte.Photometrie.png )

For a detailed description of this software see: https://astro.stroblhofwarte-oberrohrbach.de (german only)
