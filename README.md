# Unity WaveFunctionCollapse

A fork of [https://github.com/mxgmn/WaveFunctionCollapse](https://github.com/mxgmn/WaveFunctionCollapse) with tools for the Unity Game engine.

# Installation

Clone this repo under your projets `Assets` directory, or import a unity package asset from
[http://selfsame.itch.io/unitywfc](http://selfsame.itch.io/unitywfc).

# Usage

## video tutorial

[https://www.youtube.com/watch?v=CTJJrC3BAGM](https://www.youtube.com/watch?v=CTJJrC3BAGM)


## Training

Training components define sample data from their child objects.  The contained objects must have a prefab connection. Rotation of objects are recorded.  The OverlapWFC component will auto compile it's training on Start, in editor mode you will need to use the `compile` button.  

Whitespace (no object) is recorded for the OverlapWFC, but ignored for the SimpleTiledWFC.

NOTE: for SimpleTiledWFC all prefabs must be located within an `Assets/Resources` directory!

## TilePainter

Simple tilemap painting utility to help create training data.  To paint, assign it's `color` prefab property and hover over the canvas region with the TilePainter's object selected.

Adding objects to the `palette` array will show them below the canvas area, palette or painted tiles can be sampled by holding the [s] key and clicking on them.

Note: You can drag a folder of prefabs from under `Assets/Resources` onto the palette array!


## OverlapWFC

Generates output from a Training sample.  On `Start` will compile it's training component, generate, and run.

NOTE: Using rotation specific tiles will only give nice results for symmetry 1 generation.  Personally I like to use rotation for the overlap model, with a larger training area to make up for the loss of symetry variants.

* seed: `0` for randomized
* N: size of the overlap patterns (this is hidden in the inspector, as higher values can often freeze Unity)
* Periodic Input: repeating sample pattern
* Periodic Output: repeating output pattern
* Symmetry: sample grid re-read with additional rotation/reflection variations. Note: rotation of sample tiles will only make sense for symmetry-1 output
* Iterations: 0 will run until finished/unfinishable. Also used by incremental output.
* Incremental: Runs iterations every update in play mode.

## SimpleTiledWFC

This model uses XML data representing legal tile neighbors.  The Training component has a "record neighbors" command to generate these files, the file is saved to `"Assets/{{trainingGameObject.name}}.xml"`.

Whitespace is ignored in the neighbor scan, and can be used as margins to isolate neighbors.  Training components have a `weight` array for neighbor xml.

NOTE: Prefabs must be located within `Assets/Resources` or a subdirectory within. 

### `X` `I` `T` `L`

Tile symmetry class can be declared via the last letter of the prefab name, for example "GroundX" or "Road-T".

Note: `L` tiles have an initial orientation of 
```
OO
OX
```

## API

Both models have a `public GameObject[,] rendering;` 2d array that stores their output.