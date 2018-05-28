# Peril Physics

![Unity3d Quadtree](/screenshot.png?raw=true "Unity3d Quadtree")

A simple collision detection system.  Includes two algorithms:  
* Brute checks all bodies against all other bodies, and it can be hard on performance and only recommended for 100 or less bodies.  
* QuadTree queries a QuadTree to test collisions against nearby bodies only.  Much more performant than Brute, but takes a bit of extra work to set up.