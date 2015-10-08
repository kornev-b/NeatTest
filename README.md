# SharpNeat in use

This project contains experiments based on SharpNeat (http://sharpneat.sourceforge.net/) neuroevolution framework.  
All experiments can be found in SharpNeatDomains directory.

Classification problems are located in: /Classification.

The main important class containing a specific domain fitness function implementation should be named *Evaluator.

To build the project and launch neuroevolution domains you need to follow these steps:  
1. Install Visual Studio (Community Edition will be a good choice to work with)  
https://www.visualstudio.com/en-us/downloads/download-visual-studio-vs.aspx  
2. Open project (*.sln file) in VS.  
3. Change build type to release.  
4. Build the solution.  
5. Open a bin directory which is located in SharpNeatGui/bin/release  
6. Find the SharpNeatGui execution file and launch it.  
7. Now you can choose a specific domain from the combobox.  
8. Load default domain parameters.  
9. Create population.  
10. Start the neuroevolution process.  
11. You can view the best genome (neural network structure) if you click View->Best genome  
