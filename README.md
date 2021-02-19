# Procedural Crystal Twine

This is one of my first steps in procedural generation. Crystal twines like structures are part of my fictional world and I wanted to be able to generate them for the game I'm developing in Unity3D by myself to save a bit of time and maybe bring some variety into the game world.

The result should in a way like this basic sketch where the red dots are the vertices and the lines the edges:

<img src="https://github.com/cfloeth/ProceduralCrystalTwine/blob/main/Readme%20Images/CrystalTwine.png" alt="Basic sketch of a crystal twine" width="250"/>

## Goal ##
The crystal twines should be fully procedurally generated in these details:

- twine node path (node count, node orientations etc.)
- vertex count in each node neighbourhood (the mesh is divided into ring areas near each node)
- vertex position offsets
- material
- hopefully branches later on

The twine should look organic.
