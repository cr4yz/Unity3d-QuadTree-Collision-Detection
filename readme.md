# Collision Detection & QuadTree

![Unity3d Quadtree](/screenshot.png?raw=true "Unity3d Quadtree")

Demo video: https://crayz.tv/files/sharex/2018-05-28%2017-19-29.mp4

A simple collision detection system.  Includes two algorithms:  
* Brute checks all bodies against all other bodies, and it can be hard on performance and only recommended for 100 or less bodies.  
* QuadTree queries a QuadTree to test collisions against nearby bodies only.  Much more performant than Brute, but takes a bit of extra work to set up.