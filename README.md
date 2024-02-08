# Graph-Weighted Wave Function Collapse
 
This repository contains a Unity implementation of an experimental extension to the Wave Function Collapse algorithm in which each cell collapse is impacted by proximity to a weighted graph. 

The goal of modifying the algorithm was to allow for greater control over map generation, such that a general structure and paths could be guaranteed. This is achieved by the probability of each tile being weighted based on proximity to its closest edge from the weighted graph. Thus, tiles considered 'walkable' will be more likely to be selected closer to an edge, shaping the output to more closely match the underlying graph.

This implementation was made primarily for the process of seeing if the concept worked in any form, so it is lacking in several areas.

For more info on base Wave Function Collapse, check out https://github.com/mxgmn/WaveFunctionCollapse.
Other references are contained in the research paper.

## Examples of output

| Non Weighted | Exponentially Weighted |
| ----------- | ----------- |
| <div align="center"> ![Picture1](https://github.com/cazzerty/GraphWeightedWaveFunctionCollapse/assets/61497672/e0b8cc86-1fc9-490e-816b-bd8aaa6609f0) </div> | <div align="center"> ![Picture2](https://github.com/cazzerty/GraphWeightedWaveFunctionCollapse/assets/61497672/7fd00898-33b2-4f5e-8806-852c1c0a427e) </div> |
| ![Picture3](https://github.com/cazzerty/GraphWeightedWaveFunctionCollapse/assets/61497672/faea4cbb-8fd6-4fce-afe6-33d71b180642)  | ![Picture4](https://github.com/cazzerty/GraphWeightedWaveFunctionCollapse/assets/61497672/fd5c4321-6b20-4b83-bdf3-3968cf1629ca) |

## Research Paper
**Further information about the project can be found in the submitted paper**

[GraphWeightedWaveFunctionCollapse.pdf](https://github.com/cazzerty/GraphWeightedWaveFunctionCollapse/files/14206100/GraphWeightedWaveFunctionCollapse.pdf)

_This project was a 2023 UTS Engineering Research Capstone_
