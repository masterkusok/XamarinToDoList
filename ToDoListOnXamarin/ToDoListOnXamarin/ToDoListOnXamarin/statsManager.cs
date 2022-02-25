using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ToDoListOnXamarin
{
    class statsManager
    {
        public struct tasksDoneInfo
        {
            public DateTime date { get; set; }
            public int tasksDone { get; set; }
            public int tasksWere { get; set; }
        }
        
        public List<tasksDoneInfo> allDoneTasksInfo = new List<tasksDoneInfo>();
        string backingFile;

        int tasksWere;
        int tasksDone;

        public statsManager(int done, int were)
        {
            tasksDone = done;
            tasksWere = were;
            backingFile = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "stats.json");
            parseJsonForInfos();
            checkOutOfDateInfos();

            editTodayInfo(done, were);
            
        }

        public int getDoneThisMonth()
        {
            int numberOfDoneTasks = 0; ;
            foreach(tasksDoneInfo inf in allDoneTasksInfo)
            {
                if(inf.tasksDone == inf.tasksWere && inf.tasksWere != 0)
                {
                    numberOfDoneTasks++;
                }
            }
            return numberOfDoneTasks;
        }

        private void SyncInfosWithJson()
        {
            File.WriteAllText(backingFile, JsonSerializer.Serialize(allDoneTasksInfo));
        }

        private void editTodayInfo(int done, int were)
        {
            bool found = false;
            List<tasksDoneInfo> tempDoneTasksInfo = new List<tasksDoneInfo>();
            foreach (tasksDoneInfo inf in allDoneTasksInfo)
            {
                if (inf.date.Date == DateTime.Now.Date)
                {
                    found = true;
                    tasksDoneInfo temp = new tasksDoneInfo { date = DateTime.Now.Date, tasksDone = done, tasksWere = were };
                    tempDoneTasksInfo.Add(temp);
                }
                else
                {
                    tempDoneTasksInfo.Add(inf);
                }

            }
            if (!found)
            {
                tempDoneTasksInfo.Add(new tasksDoneInfo { date = DateTime.Now.Date, tasksDone = done, tasksWere = were });
            }
            allDoneTasksInfo = tempDoneTasksInfo;
            SyncInfosWithJson();
        }

        private void checkOutOfDateInfos()
        {
            List<tasksDoneInfo> tempDoneTasksInfo = new List<tasksDoneInfo>();
            foreach (tasksDoneInfo inf in allDoneTasksInfo)
            {
                if((DateTime.Today.Date - inf.date.Date).TotalDays < 30) {
                    tempDoneTasksInfo.Add(inf);
                }
            }
            allDoneTasksInfo = tempDoneTasksInfo;
        }

        private void parseJsonForInfos()
        {
            //File.Delete(backingFile);
            if (File.Exists(backingFile))
            {
                string json_string = File.ReadAllText(backingFile);
                if (!String.IsNullOrEmpty(json_string))
                {
                    allDoneTasksInfo = JsonSerializer.Deserialize<List<tasksDoneInfo>>(File.ReadAllText(backingFile));
                }
            }
            else
            {
                using(StreamWriter SW = new StreamWriter(backingFile))
                {
                    //allDoneTasksInfo.Add(new tasksDoneInfo { tasksDone = tasksDone, tasksWere = tasksWere, date = DateTime.Now.Date });
                    SW.Write(JsonSerializer.Serialize(allDoneTasksInfo));
                    SW.Close();
                }
            }
        }

    }
}
