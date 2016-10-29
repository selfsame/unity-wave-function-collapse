# Unity WaveFunctionCollapse

A fork of [https://github.com/mxgmn/WaveFunctionCollapse](https://github.com/mxgmn/WaveFunctionCollapse) for the Unity Game engine

# Usage

## Training

Training components define sample data from their child objects.  The contained objects must have a prefab connection. Recompile after changing its contents.  Rotation of objects are recorded.

## OverlapWFC

Generates output from a Training sample.  On `Start` will compile it's training component, generate, and run.

* seed: `0` for randomized
* N: size of the overlap patterns (this is hidden in the inspector, as higher values can often freeze Unity)
* Periodic Input: repeating sample pattern
* Periodic Output: repeating output pattern
* Symmetry: sample grid re-read with additional rotation/reflection variations. Note: rotation of sample tiles will only make sense for symmetry-1 output
* Iterations: 0 will run until finished/unfinishable. Also used by incremental output.
* Incremental: Runs iterations every update in play mode.


## TilePainter

Simple painting utility.  Add prefabs to it's `palette` array, then select the TilePainter's gameobject and use the mouse in the editor window to paint/interact.

Note: use the sample key [S] + click to change paint color, either from the canvas area or the row of palette objects below it.


## SimpleTiledWFC

This model uses XML data representing legal tile neighbors.  The Training component has a "record neighbors" command to generate these files, the file is saved to `"Assets/{{trainingGameObject}}.xml"`.

Whitespace is ignored in the neighbor scan, and can be used as margins to isolate neighbors.  Training components have a `weight` array for neighbor xml.

NOTE: Prefabs must be located within `Assets/Resources` or a subdirectory within. 

### `X` `I` `T` `L` `/`

Tile symmetry class can be declared via the last letter of the prefab name, for example "GroundX" or "Road-T".

Note: `L` tiles have an initial orientation of 
```
OO
OX
```

### Training 
<img src="https://cloud.githubusercontent.com/assets/2467644/19320599/ea67a16e-907f-11e6-987e-5cb34ad60cf3.gif">
### TilePainter
<img src="https://cloud.githubusercontent.com/assets/2467644/19320600/eb9c2406-907f-11e6-8750-f18619a7fc6b.gif">
### SimpleTiledWFC
<img src="https://cloud.githubusercontent.com/assets/2467644/19320603/ec872780-907f-11e6-89fc-dd67fac33069.gif">