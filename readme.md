# WFCPROCGEN
this is an attempt to implement wave function collapse with unity.  
it's not very good and needs abstracted out.  

## Goals
- [x] implement basic wfc
- [x] (kind of done) generate new tilemaps from existing tilemaps - this is broken but the concept is there
- [ ] (1/3 done) use bounding box to march the edge of a tilemap to generate a new tilemap that has edge matching so the transitions are seamless
- [x] refactor this shit so it's not one big dumb file with no real structure
- [ ] generate new tilemaps in a boundary box around the camera (infinite generation)
- [ ] (working on it) store generated tilemaps in some sort of data structure
- [ ] frustum culling to remove tilemaps that are too far away to see
- [ ] Get the fuck out of unity and implement this in C++
