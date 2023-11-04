## Ordered Action Sequences

This was originally created to better coordinate powerful operations in perfect sequence. A good example is when a character goes out of bounds and needs to be reset. 

-The player's controls should be frozen, then 
- the screen should fade to black.
- While not visible, move the player to a safe position and adjust the camera angle.
- Once thats finished, fade back in to view the player in the new position.
- Finally, unfreeze controls.

Thats a lot of very specific code to write for a very specific use case. Most of those things such as fading the camera to black, positioning the player and toggling controls will almost certainly be used elsewhere. 
Rather than creating complicated controllers to do these specific action in every one-off use case, you can create this abstract "sequence" that is "run". The controller doesnt need to know all the details, just to run a sequence.

### Configuration
TODO
