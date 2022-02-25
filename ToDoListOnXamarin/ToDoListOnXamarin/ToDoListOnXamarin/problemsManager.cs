using System;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;

namespace ToDoListOnXamarin
{
    public class problemsManager
    {
        public class Problem
        {
            public string problem_text { get; set; }
            public string repeat_type { get; set; }
            public bool is_done { get; set; }
            public DateTime created_date { get; set; }
        }

        public problemsManager()
        {
            backingFile = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "problems.json");
        }

        bool abcdefg = true;
        public List<Problem> allProblems = new List<Problem>();
        string backingFile;

        //this method deletes problem from list
        public void deleteProblem(int number)
        {
            allProblems.Remove(allProblems[number]);
            string json_string = JsonSerializer.Serialize(allProblems);
            File.WriteAllText(backingFile, json_string);
        }

        //this method edits problem from list
        public void EditProblem(int number, string text, string repeat_type, bool done)
        {
            Problem EditedProblem = new Problem 
            { problem_text = text, repeat_type = repeat_type, is_done = done, created_date = DateTime.Now };
            allProblems[number] = EditedProblem;
            string json_string = JsonSerializer.Serialize(allProblems);
            File.WriteAllText(backingFile, json_string);

        }
        //this method adds new Problem to list
        public void addProblem(string text, string repeat_type)
        {
            Problem createdProblem =
                new Problem { problem_text = text, repeat_type = repeat_type, is_done = false, created_date = DateTime.Now };
            allProblems.Add(createdProblem);
            string json_string = JsonSerializer.Serialize(allProblems);
            File.WriteAllText(backingFile, json_string);
        }

        public void checkProblemsOutOfDate()
        {
            List<Problem> temp_allProblems = new List<Problem>();
            for(int i = 0; i < allProblems.Count; i++)
            {
                if(allProblems[i].created_date.Date < DateTime.Now.Date)
                {
                    if (allProblems[i].repeat_type == "everyday")
                    {
                        Problem temp_problem = allProblems[i];
                        temp_problem.created_date = DateTime.Now;
                        temp_problem.is_done = false;
                        temp_allProblems.Add(temp_problem);
                    }
                    
                }
                else
                {
                    temp_allProblems.Add(allProblems[i]);
                }
            }
            allProblems = temp_allProblems;

            string json_string = JsonSerializer.Serialize(allProblems);
            File.WriteAllText(backingFile, json_string);
        }

        public void getProblems()
        {
            if (File.Exists(backingFile))
            {
                Console.WriteLine(backingFile);
                using (StreamReader SR = new StreamReader(backingFile)) {
                    string string_text_fromJson = SR.ReadToEnd();
                    if (!String.IsNullOrEmpty(string_text_fromJson))
                    {
                        allProblems = JsonSerializer.Deserialize<List<Problem>>(string_text_fromJson);
                    }
                }
                
            }
            else
            {
                using(StreamWriter SW = new StreamWriter(backingFile))
                {
                    SW.Write("");
                    SW.Close();
                }
                JsonSerializer.Serialize(allProblems);
            }
        }
    }
}
