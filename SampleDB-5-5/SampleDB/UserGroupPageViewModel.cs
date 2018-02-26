using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace SampleDB
{
    public class UserGroupPageViewModel :INotifyPropertyChanged
    {

        public class JSSEInfo
        {
            public int jsseId { get; set; }
            public string jsseStatus { get; set; }
            public string jsseDate { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Group> GroupItems { get; set; }



        public int SelectedGroupId;


        private ObservableCollection<JSSEInfo> _listeviewItemSource = new ObservableCollection<JSSEInfo>();

        public ObservableCollection<JSSEInfo> ListeviewItemSource
        {
            get { return _listeviewItemSource; }
        }


        public UserGroupPageViewModel()
        {

            var assembly = typeof(DynamicScreen).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("SampleDB.UserGroup.json");
            using (var reader = new System.IO.StreamReader(stream))
            {

                var json = reader.ReadToEnd();
                UserGroupModel userGroupModel = JsonConvert.DeserializeObject<UserGroupModel>(json);

                List<Group> sortingGroup = new List<Group>();
                for (int i = 0; i < userGroupModel.Groups.Count; i++)
                {
                    if (userGroupModel.Groups[i].GroupType.Level_Id == 2)
                    {
                        sortingGroup.Add(userGroupModel.Groups[i]);
                    }
                }

                //pickerRef.ItemsSource = sortingGroup;
                GroupItems = sortingGroup;
            }


            JSSEInfo obj = new JSSEInfo();
            obj.jsseId = 510;
            obj.jsseStatus = "Draft";
            obj.jsseDate = "09/20/2017";


            JSSEInfo obj1 = new JSSEInfo();
            obj1.jsseId = 511;
            obj1.jsseStatus = "Submit";
            obj1.jsseDate = "09/20/2017";


            JSSEInfo obj2 = new JSSEInfo();
            obj2.jsseId = 512;
            obj2.jsseStatus = "Submit";
            obj2.jsseDate = "09/20/2017";


            JSSEInfo obj3 = new JSSEInfo();
            obj3.jsseId = 513;
            obj3.jsseStatus = "Draft";
            obj3.jsseDate = "09/20/2017";


            JSSEInfo obj4 = new JSSEInfo();
            obj4.jsseId = 514;
            obj4.jsseStatus = "Submit";
            obj4.jsseDate = "09/20/2017";


            JSSEInfo obj5 = new JSSEInfo();
            obj5.jsseId = 515;
            obj5.jsseStatus = "Draft";
            obj5.jsseDate = "09/20/2017";


            ListeviewItemSource.Add(obj);
            ListeviewItemSource.Add(obj1);
            ListeviewItemSource.Add(obj2);
            ListeviewItemSource.Add(obj3);
            ListeviewItemSource.Add(obj4);
            ListeviewItemSource.Add(obj5);

            
        }

        Group selectedGroup;
        public Group SelectedGroup
        {
            get { return selectedGroup; }
            set
            {
                if (selectedGroup != value)
                {
                    selectedGroup = value;
                    SelectedGroupId = selectedGroup.Group_ID;
                    OnPropertyChanged("selectedGroup");
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
