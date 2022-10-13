# Modelling

## Short description
The idea of the game is to build the model as close as possible to target model. Model consists of tiny voxels. Optimal ammount of voxels per model is 27 million, so for Unity it's relatively easy to render.

## Flow
Target model is randomly generated among predefined shapes  - Sphere, Ellipsoid, Perlin Noise.  
You can cut out or build up model using different "brushes". Currently available - spherical and cubic. Also you possibility to change the brush size.  
How close you are to target model is displayed in the upper center of the sreen.

![Screenshot (26)](https://user-images.githubusercontent.com/74894929/195610844-7df00b27-2192-430c-b665-446f20783acd.png)

## Controls

Cut out (Intrude) - Left mouse button  
Build up (Extrude) - Right mouse button  
Undo action - Ctrl + Z  
