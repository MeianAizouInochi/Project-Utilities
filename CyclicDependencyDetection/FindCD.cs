using System;
using System.IO;
using System.Collections.Generic;


namespace CyclicDependencyDetection
{
    /// <summary>
    /// This class helps to find all Cyclic Dependecy in a provided graph.
    /// </summary>
    public class FindCD
    {
        //Adjacency list of Vertex
        private Dictionary<string, string[]> AdjMap;

        //Marker for checking if Vrtex is already visited.
        private Dictionary<string,bool> VisitMarker;

        //Vertex Visiting route or Vertex stack
        private List<string> VisVertex;

        //Constructor
        public FindCD()
        {
            //Initialize all the Fields
            AdjMap = new Dictionary<string, string[]>();
            VisitMarker = new Dictionary<string, bool>();
            VisVertex = new List<string>();
        }


        static void Main(string[] args)
        {
            //Create instance
            FindCD solution = new FindCD();

            //get filename from input from cmd line args.
            if(args.Length==0)
            {
                Console.WriteLine("No input file Path provided!");
                Environment.Exit(0); //Exit if path not provided.
            }

            string path = args[0];

            //Checking Existence of the file.
            if(File.Exists(path))
            {
               try
                {
                    //Reading from the file and performing required actions.
                    using(StreamReader sr = new StreamReader(path))
                    {
                        string line = "";

                        //populating AdjMap or Adjacency List from lines of the file.
                        while((line = sr.ReadLine())!=null)
                        {
                            string[] pair = line.Split(':');
                            
                            if(pair[1].Length!=0)
                            {
                                string[] adjV = pair[1].Split(',');

                                solution.AdjMap.Add(pair[0],adjV);
                            }
                            else
                            {
                                solution.AdjMap.Add(pair[0],new string[0]);
                            }
                            
                            solution.VisitMarker.Add(pair[0],false);
                        }

                        //Find all Cycles.
                        List<string> result = solution.FindAllCycles();
                        

                        //Removing Duplicate Cycles. NOTE: ClockWise rotation Cycle Duplicates haven't removed by this.
                        Dictionary<string,int> UnDuplicated = new Dictionary<string,int>();

                        foreach(string i in result)
                        {
                            if(!UnDuplicated.ContainsKey(i))
                            {
                                UnDuplicated.Add(i,1);
                            }
                            else
                            {
                                UnDuplicated[i] +=1;
                            }
                        }
                        
                        //Display result.
                        foreach(string i in UnDuplicated.Keys)
                        {
                            Console.WriteLine(i);
                        }

                        Console.WriteLine("Finished!");
                    }
                }
                catch(IndexOutOfRangeException e)
                {
                    Console.WriteLine("Please recheck the text in the Input file!");
                    Environment.Exit(0);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    Environment.Exit(0);

                }
                //provide more exception cases.  
            }
            else
            {
                Console.WriteLine("File doesnt Exists!");
                Environment.Exit(0);
            }
        }

        private List<string> FindAllCycles()
        {
            //result List of Cycles Found in the graph.
            List<string> result = new List<string>();
            
            //Iterate through each vertex and do a dfs on each vertex.
            foreach(string i in AdjMap.Keys)
            {
                result.AddRange(RecursiveDFS(i)); //Adding the Cycles to the result.

                VisitMarker[i]= false; //Mark the marker false.

                VisVertex.RemoveAt(0); //Remove from the visiting route.
            }

            return result;
        }
        private List<string> RecursiveDFS(string Vertex)
        {
            Console.WriteLine( "Vertex: " + Vertex);

            //result found corresponding to this vertex.
            List<string> res = new List<string>();

            //If vertex was previously visited, then marker should be marked true
            if(VisitMarker[Vertex] == true) //This means a cycle is detected.
            {
                VisitMarker[Vertex] = !VisitMarker[Vertex]; //Change Marker to false.
                
                /*
                    Getting the string of the cycle route.
                */
                string resS = ":"+ Vertex;

                VisVertex.Add(Vertex);

                int index = VisVertex.Count-2;

                while(!VisVertex[index].Equals(Vertex))
                {
                    resS = ":" + VisVertex[index] + resS;
                    index--;
                }

                resS = Vertex + resS;

                Console.WriteLine(resS);

                res.Add(resS);

                return res;

            }
            else  //if not a cycle.
            {
                VisitMarker[Vertex] = true; //Mark marker to rue.
                
                VisVertex.Add(Vertex); //Add to Visiting route.

                if(AdjMap[Vertex].Length==0) //If got no Connections.
                {
                    return res;
                }
                else
                {
                    for(int index = 0; index < AdjMap[Vertex].Length; index++)
                    {
                        res.AddRange(RecursiveDFS((AdjMap[Vertex])[index]));    //Recursive Call.

                        VisitMarker[(AdjMap[Vertex])[index]] = !VisitMarker[(AdjMap[Vertex])[index]];

                        VisVertex.RemoveAt(VisVertex.Count-1);

                        Console.WriteLine(Vertex + " Current Visit State :  " + VisitMarker[Vertex]);

                        Console.WriteLine((AdjMap[Vertex])[index] + " Current Visit State :  " + VisitMarker[(AdjMap[Vertex])[index]]);

                        Console.WriteLine("Current end point:" + VisVertex[VisVertex.Count-1]);

                    }
                    return res;
                }


            }

        }

    }
}