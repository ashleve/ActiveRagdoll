# ActiveRagdoll
#### Implementing active ragdoll from scratch for humanoid characters in Unity

![](gifs/active_ragdoll.gif)

## What is this?
#### This is a simple third person controller for active ragdoll driven by static animations.

It uses similar techniques as games like [TABS](https://www.youtube.com/watch?v=Z2e9vd3Znz4) or [Human Fall Flat](https://www.youtube.com/watch?v=-Edk59BqSEU).

It's designed to be as simple as possible.

![](gifs/balls.gif)

## How does it work??
#### TL;DR: a [PID controller](https://en.wikipedia.org/wiki/PID_controller) applies forces to ragdoll to make him follow static animator (skeleton in the gif below).

[`AnimationFollowing.cs`](https://github.com/hobogalaxy/UnityActiveRagdoll/blob/master/Assets/ActiveRagdollScripts/AnimationFollowing.cs) applies animation following.

[`MasterController.cs`](https://github.com/hobogalaxy/UnityActiveRagdoll/blob/master/Assets/ActiveRagdollScripts/MasterController.cs) contains all 3rd person controller logic. It makes sure static animator can't move too far away from ragdoll.

[`SlaveController.cs`](https://github.com/hobogalaxy/UnityActiveRagdoll/blob/master/Assets/ActiveRagdollScripts/SlaveController.cs) controls behaviour of animation following in real time. It makes ragdoll loose strength when colliding with other objects.

![](gifs/skielete.gif)

*Work in progress.*
