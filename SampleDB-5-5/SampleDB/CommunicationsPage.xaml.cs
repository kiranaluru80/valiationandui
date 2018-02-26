using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using static SampleDB.SaveJsseModel;
using System.Linq;

namespace SampleDB
{
    public partial class CommunicationsPage : ContentPage
    {
        

        public List<JSSEMasterCategory> listOfCategarys;
		public List<RatingTable> listOfRatings;
        public List<JSSEMasterBehavior> listOfEnterprisePerCategary;
        public List<JSSEMasterBehavior> listOfBehaviersPerCategary;

        public List<Categories> InputCategories;
        public List<EntBehavior> InputOrgBehaviors;

        public int _Org_Id;
        public static Editor SelectedEditor;

        public string selectedGridLabelName;

        public CommunicationsPage(int Org_Id,string jsseid)
        {
            InitializeComponent();

            selectedGridLabelName = "";

            this.Title = jsseid;

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (s, e) => {
                // handle the tap
                DisplayAlert("", "JOB Information", "OK");
                Navigation.PushAsync(new JobInformationPage());
            };
            jobInformationRef.GestureRecognizers.Add(tapGestureRecognizer);
            tapGestureRecognizer.NumberOfTapsRequired = 1;


            saveBtnRef.Clicked += SaveBtnRef_Clicked;

            InputCategories = new List<Categories>() ;
            InputOrgBehaviors = new List<EntBehavior>();
            _Org_Id = Org_Id;


            ClearButtonRef.Clicked += (sender, e) => {
                if (SelectedEditor != null)
                {
                    SelectedEditor.Text = "";
                    CommentsEditorref.Text = "";
                    //CommentsView.IsVisible = true;
                }
            };

            ContinueButtonref.Clicked += (sender, e) => {
                if(SelectedEditor != null){
                    saveBtnRef.IsEnabled = true;
                    if (!string.IsNullOrWhiteSpace(CommentsEditorref.Text))
                    {
                        CommentsView.IsVisible = false;
                        SelectedEditor.Text = CommentsEditorref.Text;
                        var selectedStyleId = SelectedEditor.StyleId;
                        string[] words = selectedStyleId.Split(' ');
                        if(words.Count() == 2){
                            EntBehavior entBehavior = new EntBehavior();
                            entBehavior.Category_ID = Int32.Parse(words[0]);
                           // entBehavior.Rating_ID = Convert.ToInt32(selectedratingId);
                            entBehavior.Behavior_ID = Int32.Parse(words[1]);
                            entBehavior.Comments = CommentsEditorref.Text;

                            List<EntBehavior> SortedResponceDataOne = InputOrgBehaviors.Where(x => x.Category_ID == Int32.Parse(words[0]) && x.Behavior_ID == Int32.Parse(words[1])).ToList();
                            if (SortedResponceDataOne.Count == 0)
                            {
                                InputOrgBehaviors.Add(entBehavior);
                            }
                            else
                            {
                                for (int h = 0; h < InputOrgBehaviors.Count; h++)
                                {
                                    EntBehavior obj = InputOrgBehaviors[h];
                                    if (obj.Category_ID == Int32.Parse(words[0]) && obj.Behavior_ID == Int32.Parse(words[1]))
                                    {

                                        InputOrgBehaviors[h].Category_ID = Int32.Parse(words[0]);
                                        InputOrgBehaviors[h].Comments = CommentsEditorref.Text;
                                        InputOrgBehaviors[h].Behavior_ID = Int32.Parse(words[1]);
                                        InputOrgBehaviors[h].Rating_ID = obj.Rating_ID;
                                    }
                                }
                            }



                        }else{
                            
                            Categories categoriesObj = new Categories();
                            categoriesObj.Comments = CommentsEditorref.Text;
                            categoriesObj.Category_ID = Int32.Parse(words[0]);

                            List<Categories> SortedResponceDataOne = InputCategories.Where(x => x.Category_ID == Int32.Parse(words[0])).ToList();
                            if (SortedResponceDataOne.Count == 0)
                            {
                                InputCategories.Add(categoriesObj);
                            }
                            else
                            {
                                for (int h = 0; h < InputCategories.Count; h++)
                                {
                                    Categories obj = InputCategories[h];
                                    if (obj.Category_ID == Int32.Parse(words[0]))
                                    {

                                        InputCategories[h].Category_ID = Int32.Parse(words[0]);
                                        InputCategories[h].Comments = categoriesObj.Comments;
                                        InputCategories[h].RatingID = obj.RatingID;
                                    }
                                }
                            }
                        }
                    }else{
                        CommentsView.IsVisible = false;
                        StackLayout layout = SelectedEditor.Parent as StackLayout;
                        layout.IsVisible = false;
                    }
                }
            };

        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

           // listOfObjects = await App.Database.GetJsseBehaviors(0,1);
			await ReadDataFromJson();

            listOfCategarys = await App.Database.GetJsseCategories();
            listOfRatings = await App.Database.GetRatings();
           // listOfObjectsPerCategary = await App.Database.GetJsseBehaviors(0, 1);
            DynamicCategaryDesign(listOfCategarys);

        }


        public async void DynamicCategaryDesign(List<JSSEMasterCategory> listOfCategarys){


            for (int i = 0; i < listOfCategarys.Count; i++)
            {
                StackLayout mainStackLayout = new StackLayout();
                mainStackLayout.Padding = new Thickness(20, 0, 20, 0);
                mainStackLayout.Orientation = StackOrientation.Horizontal;
                mainStackLayout.Spacing = 20;
                mainStackLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
                mainStackLayout.VerticalOptions = LayoutOptions.Start;
                mainStackLayout.BindingContext = listOfCategarys[i];
                mainStackLayout.BackgroundColor = Color.Beige;
                mainStackLayout.StyleId = "SelectedItem";
               // mainStackLayout.Margin = new Thickness(0, 5, 0, 5);
				
				Label titleLabel = new Label();
                titleLabel.Text = listOfCategarys[i].Category;
                titleLabel.HorizontalOptions = LayoutOptions.FillAndExpand;
                titleLabel.VerticalOptions = LayoutOptions.Center;
                titleLabel.FontSize = 14;
                titleLabel.TextColor = Color.Black;
                titleLabel.FontAttributes = FontAttributes.Bold;
                titleLabel.Margin = new Thickness(0, 12, 0, 12);

                Image selectedImageorNot = new Image();
                selectedImageorNot.HorizontalOptions = LayoutOptions.End;
                selectedImageorNot.VerticalOptions = LayoutOptions.Center;
                selectedImageorNot.HeightRequest = 20;
                selectedImageorNot.WidthRequest = 20;
                selectedImageorNot.BackgroundColor = Color.Transparent;

                Image arrowImage = new Image();
                arrowImage.Source = "ArrowDown";
                arrowImage.HorizontalOptions = LayoutOptions.End;
                arrowImage.VerticalOptions = LayoutOptions.Center;
                arrowImage.HeightRequest = 20;
                arrowImage.WidthRequest = 20;

                mainStackLayout.Children.Add(titleLabel);
                mainStackLayout.Children.Add(selectedImageorNot);
                mainStackLayout.Children.Add(arrowImage);




                BoxView bottomLine = new BoxView();
                bottomLine.HorizontalOptions = LayoutOptions.FillAndExpand;
                bottomLine.VerticalOptions = LayoutOptions.Start;
                bottomLine.HeightRequest = 1;
                bottomLine.BackgroundColor = Color.Silver;


                CategaryStacklayout.Children.Add(mainStackLayout);
                CategaryStacklayout.Children.Add(bottomLine);
                CategaryStacklayout.StyleId = listOfCategarys[i].Category;
                //CategaryStacklayout.BackgroundColor = Color.Beige;

				var communicationTapGestureRecognizer = new TapGestureRecognizer();
                communicationTapGestureRecognizer.Tapped +=  (sender, e) => {

                    StackLayout selectedLayout = sender as StackLayout;
                    Label selectedLabelString = selectedLayout.Children[0] as Label;
                    selectedGridLabelName = selectedLabelString.Text;
                    StackLayout selecteLayoutParent = selectedLayout.Parent as StackLayout;
                    var selectedObject = selectedLayout.Children.Count;
                    var countVal = selecteLayoutParent.Children.Count;
                    // var obj = visibleOrInvisible.StyleId;
                    //StackLayout selectedStackChildern = selectedLayout.Parent.FindByName<StackLayout>("isVisibleOrInVisibleRef");

                    //listOfObjectsPerCategary = await App.Database.GetJsseBehaviors(0, 1);



                    for (int j = 0; j < selecteLayoutParent.Children.Count; j++){



                        if (selecteLayoutParent.Children[j].StyleId != null)
                        {
                            if (selecteLayoutParent.Children[j].StyleId == "SelectedItem")
                            {
                                StackLayout stackRef = selecteLayoutParent.Children[j] as StackLayout;
                                Image imageRef = stackRef.Children[2] as Image;
                                imageRef.Source = "ArrowDown";
                            }

                            Label selectedLabel = selectedLayout.Children[0] as Label;
                            Image selectedImage = selectedLayout.Children[2] as Image;
                           // selectedImage.Source = "ArrowDown";

                            if (selectedLabel.Text == selecteLayoutParent.Children[j].StyleId)
                            {
                                if (selecteLayoutParent.Children[j].IsVisible)
                                {
                                    selecteLayoutParent.Children[j].IsVisible = false;
                                    selectedImage.Source = "ArrowDown";

                                     Device.BeginInvokeOnMainThread(() =>
                                     {
                                        scrollViewRef.ScrollToAsync(0, 0, true);
                                     });
                                   

								}
                                else
                                {
                                    selecteLayoutParent.Children[j].IsVisible = true;
                                    selectedImage.Source = "Arrowup";

                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        //scrollViewRef.ScrollToAsync(0, 0, true);
                                        scrollViewRef.ScrollToAsync(selectedLayout, ScrollToPosition.Start, true);
                                    });
                                  
                                  //  scrollViewRef.ScrollToAsync(selectedImage, ScrollToPosition.Start, true);
                                }
                            }
                            else if (selecteLayoutParent.Children[j].IsVisible && selecteLayoutParent.Children[j].StyleId != "SelectedItem")
                                {
                                selecteLayoutParent.Children[j].IsVisible = false;
                               //// Device.BeginInvokeOnMainThread(() =>
                               // //{
                                   // selectedImage.Source = "ArrowDown";
                               //// });
                                }
                        }

                        //else{
                        //    if (j == 2 || j == 5 || j == 8 || j == 11 || j == 14 || j == 17 )
                        //    {
                        //        selecteLayoutParent.Children[j].IsVisible = false;
                        //        selectedImage.Source = "Arrowup";
                        //    }
                        //}
                    }
                    JSSEMasterCategory sec = selectedLayout.BindingContext as JSSEMasterCategory;

				};
				mainStackLayout.GestureRecognizers.Add(communicationTapGestureRecognizer);
				communicationTapGestureRecognizer.NumberOfTapsRequired = 1;

                listOfEnterprisePerCategary = await App.Database.GetJsseBehaviors(0, listOfCategarys[i].Category_ID);
                listOfBehaviersPerCategary = await App.Database.GetJsseBehaviors(720, listOfCategarys[i].Category_ID);

                VisibleOrInVisibleScreenDesign(CategaryStacklayout, listOfCategarys[i].Category, listOfCategarys[i].Category_ID);
            }
        }

        public void VisibleOrInVisibleScreenDesign(StackLayout categaryStacklayoutRef, string styleIdRef, int mainCategaryId) {

            StackLayout visibleOrInVisibleMainStack = new StackLayout();
            visibleOrInVisibleMainStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            visibleOrInVisibleMainStack.VerticalOptions = LayoutOptions.Start;
            visibleOrInVisibleMainStack.Orientation = StackOrientation.Vertical;
            visibleOrInVisibleMainStack.Spacing = 0;
            visibleOrInVisibleMainStack.IsVisible = false;
            visibleOrInVisibleMainStack.StyleId = styleIdRef;
            //visibleOrInVisibleMainStack.BackgroundColor = Color.Red;

            StackLayout overAllRatingStack = new StackLayout();
            overAllRatingStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            overAllRatingStack.VerticalOptions = LayoutOptions.Start;


            StackLayout titleAndEditBtnStack = new StackLayout();
            titleAndEditBtnStack.Padding = new Thickness(20, 0, 20, 0);
            titleAndEditBtnStack.Orientation = StackOrientation.Horizontal;
            titleAndEditBtnStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            titleAndEditBtnStack.VerticalOptions = LayoutOptions.Start;
            titleAndEditBtnStack.Spacing = 5;

            Label label = new Label();
            label.HorizontalOptions = LayoutOptions.FillAndExpand;
            label.Text = "Overall rating";
            label.FontSize = 14;

            Button editButton = new Button();
            editButton.Image = "edit";
            editButton.HorizontalOptions = LayoutOptions.End;
            editButton.WidthRequest = 20;
            editButton.HeightRequest = 20;
            editButton.Clicked += (sender, e) => {
                
                Button selectedLayout = sender as Button;
                StackLayout selecteLayoutParent = selectedLayout.Parent.Parent as StackLayout;
                var selectedObject = selecteLayoutParent.Children.Count;
                var countVal = selecteLayoutParent.Children.Count;
                // var obj = visibleOrInvisible.StyleId;
                //StackLayout selectedStackChildern = selectedLayout.Parent.FindByName<StackLayout>("isVisibleOrInVisibleRef");

                //listOfObjectsPerCategary = await App.Database.GetJsseBehaviors(0, 1);
                if (selecteLayoutParent.Children.Count > 1)
                {
                    StackLayout childStackLayout = selecteLayoutParent.Children[1] as StackLayout;
                    if (childStackLayout.Children.Count == 4){

                        StackLayout editorStack = childStackLayout.Children[3] as StackLayout;
                        for (int j = 0; j < editorStack.Children.Count; j++)
                        {
                             SelectedEditor = editorStack.Children[0] as Editor;
                            editorStack.IsVisible = true;
                            CommentsView.IsVisible = true;
                            titleRefLabel.Text = "Overall rating";
                            if(SelectedEditor != null)
                               CommentsEditorref.Text = SelectedEditor.Text;
                        }
                    }

                }



            };


            titleAndEditBtnStack.Children.Add(label);
            titleAndEditBtnStack.Children.Add(editButton);
            overAllRatingStack.Children.Add(titleAndEditBtnStack);

            StackLayout ratingsLayout = new StackLayout();
            ratingsLayout.Orientation = StackOrientation.Vertical;
            ratingsLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
            ratingsLayout.VerticalOptions = LayoutOptions.Start;
            ratingsLayout.Spacing = 5;

            OverAllRatingsDesign(listOfRatings, ratingsLayout, mainCategaryId);

            overAllRatingStack.Children.Add(ratingsLayout);

            visibleOrInVisibleMainStack.Children.Add(overAllRatingStack);

            //if (listOfObjectsPerCategary != null )
            //{
                
            secondPartLayoutDesign(listOfEnterprisePerCategary, visibleOrInVisibleMainStack, "Corporate Factors", mainCategaryId);
			
            ThirdPartLayoutDesign(listOfBehaviersPerCategary, visibleOrInVisibleMainStack, "Organization Behaviors", mainCategaryId);
                categaryStacklayoutRef.Children.Add(visibleOrInVisibleMainStack);
            //}
        }

        public void secondPartLayoutDesign(List<JSSEMasterBehavior> listOfObjectsPerCategary, StackLayout secondStackRef, string titleLabel, int CategaryId){
           

            StackLayout visibleOrInvisibleStack = new StackLayout();
            visibleOrInvisibleStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            visibleOrInvisibleStack.VerticalOptions = LayoutOptions.Start;
            //visibleOrInvisibleStack.BackgroundColor = Color.Red;
            visibleOrInvisibleStack.Orientation = StackOrientation.Vertical;
            visibleOrInvisibleStack.Spacing = 0;
            visibleOrInvisibleStack.Padding =new Thickness(10);
            visibleOrInvisibleStack.StyleId = "subbbbbbbb";

            Frame frame = new Frame();
			frame.Padding = new Thickness(0,10,0,0);
			frame.OutlineColor = Color.Black;
			frame.CornerRadius = 0;
            frame.HasShadow = false;
            frame.StyleId = "hiiiiii";
            //frame.IsVisible = false;
           // frame.Margin = new Thickness(10,0,10,0);

			StackLayout mainStack = new StackLayout();
			mainStack.Orientation = StackOrientation.Vertical;
			mainStack.HorizontalOptions = LayoutOptions.FillAndExpand;
			mainStack.VerticalOptions = LayoutOptions.Start;
			mainStack.Spacing = 0;


           // StackLayout groupTitleStack = null;

            for (int i = 0; i < listOfObjectsPerCategary.Count; i++)
            {

                StackLayout groupTitleStack = new StackLayout();
                groupTitleStack.HorizontalOptions = LayoutOptions.FillAndExpand;
                groupTitleStack.VerticalOptions = LayoutOptions.Start;
                groupTitleStack.Padding = new Thickness(0);
                groupTitleStack.Spacing = 0;
                groupTitleStack.Orientation = StackOrientation.Horizontal;
                groupTitleStack.BackgroundColor = Color.Gray;
             //   groupTitleStack.BackgroundColor = Color.Green;

                StackLayout secondPartStack = new StackLayout();
                secondPartStack.HorizontalOptions = LayoutOptions.FillAndExpand;
                secondPartStack.VerticalOptions = LayoutOptions.Start;
               // secondPartStack.BackgroundColor = Color.Red;
                if (i == 0)
                {

                    Image arrowImage = new Image();
                    arrowImage.Source = "ArrowDown";
                    arrowImage.HorizontalOptions = LayoutOptions.End;
                    arrowImage.VerticalOptions = LayoutOptions.Center;
                    arrowImage.HeightRequest = 20;
                    arrowImage.WidthRequest = 20;
                    
                    //adding heading 
                    Button headingLabel = new Button();
                    headingLabel.Margin = new Thickness(20, 0, 0, 0);
                    headingLabel.HorizontalOptions = LayoutOptions.FillAndExpand;
                    headingLabel.VerticalOptions = LayoutOptions.End;
					headingLabel.Text = titleLabel;
					headingLabel.FontSize = 14;
                    headingLabel.FontAttributes = FontAttributes.Bold;
                    headingLabel.HeightRequest = 30;
                    headingLabel.TextColor = Color.White;
                   // headingLabel.BackgroundColor = Color.Red;
                    groupTitleStack.Children.Add(headingLabel);
                    groupTitleStack.Children.Add(arrowImage);

                    headingLabel.Clicked += (sender, e) => {
						//var oooo = visibleOrInvisibleStack.Children.Count;
						Button buttonRef = sender as Button;
                        StackLayout selecteLayoutParent = buttonRef.Parent.Parent as StackLayout;
                        int childCount = selecteLayoutParent.Children.Count;


                        StackLayout arrowImageparentStack = buttonRef.Parent as StackLayout;

                            Frame SelectedFrame = selecteLayoutParent.Children[selecteLayoutParent.Children.Count-1] as Frame;
                        if (SelectedFrame.IsVisible){
                            Image imageRef = arrowImageparentStack.Children[1] as Image;
                            imageRef.Source = "Arrowup";
                            SelectedFrame.IsVisible = false;
                        }else{
                            Image imageRef = arrowImageparentStack.Children[1] as Image;
                            imageRef.Source = "ArrowDown";
                            SelectedFrame.IsVisible = true;
                        }
                           // SelectedFrame.BackgroundColor = Color.Green;
                          
                    };

                    //groupTitleStack.Padding = new Thickness(0, 10, 0, 0);
                }else{
                    //groupTitleStack.Padding = new Thickness(0, 10, 0, 0);
                }

                StackLayout titleAndEditBtnStack = new StackLayout();
                titleAndEditBtnStack.Padding = new Thickness(20, 0, 20, 0);
                titleAndEditBtnStack.Orientation = StackOrientation.Horizontal;
                titleAndEditBtnStack.HorizontalOptions = LayoutOptions.FillAndExpand;
                titleAndEditBtnStack.VerticalOptions = LayoutOptions.Start;
                titleAndEditBtnStack.Spacing = 5;

                Label label = new Label();
                label.HorizontalOptions = LayoutOptions.FillAndExpand;
                label.Text = listOfObjectsPerCategary[i].Behavior;
                label.FontSize = 14;

                Button editButton = new Button();
                editButton.Image = "edit";
                editButton.HorizontalOptions = LayoutOptions.End;
                editButton.WidthRequest = 20;
                editButton.HeightRequest = 20;
                editButton.StyleId = listOfObjectsPerCategary[i].Behavior;
				editButton.Clicked += (sender, e) => {


					Button selectedLayout = sender as Button;
					StackLayout selecteLayoutParent = selectedLayout.Parent.Parent as StackLayout;
					var selectedObject = selecteLayoutParent.Children.Count;
					var countVal = selecteLayoutParent.Children.Count;
					// var obj = visibleOrInvisible.StyleId;
					//StackLayout selectedStackChildern = selectedLayout.Parent.FindByName<StackLayout>("isVisibleOrInVisibleRef");

					//listOfObjectsPerCategary = await App.Database.GetJsseBehaviors(0, 1);
					if (selecteLayoutParent.Children.Count > 1)
					{
						StackLayout childStackLayout = selecteLayoutParent.Children[selecteLayoutParent.Children.Count-1] as StackLayout;
						if (childStackLayout.Children.Count == 4)
						{

							StackLayout editorStack = childStackLayout.Children[3] as StackLayout;
							for (int j = 0; j < editorStack.Children.Count; j++)
							{
								SelectedEditor = editorStack.Children[0] as Editor;
								editorStack.IsVisible = true;
								CommentsView.IsVisible = true;
                                titleRefLabel.Text = selectedLayout.StyleId;
								CommentsEditorref.Text = SelectedEditor.Text;
							}
						}

					}



				};

                //mainStack.Children.Add(groupTitleStack);
                visibleOrInvisibleStack.Children.Add(groupTitleStack);
                //secondPartStack.Children.Add(groupTitleStack);
                //frame.Content = groupTitleStack;
                titleAndEditBtnStack.Children.Add(label);
                titleAndEditBtnStack.Children.Add(editButton);
                secondPartStack.Children.Add(titleAndEditBtnStack);

                StackLayout ratingsLayout = new StackLayout();
                ratingsLayout.Orientation = StackOrientation.Vertical;
                ratingsLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
                ratingsLayout.VerticalOptions = LayoutOptions.Start;
                ratingsLayout.Spacing = 5;

                SubRatingsDesignLayout(listOfRatings, ratingsLayout ,listOfObjectsPerCategary[i].Behavior_ID, CategaryId, listOfObjectsPerCategary[i].BehaviorType_ID);

                secondPartStack.Children.Add(ratingsLayout);
               // visibleOrInvisibleStack.Children.Add(secondPartStack);
                mainStack.Children.Add(secondPartStack);
            }
            //if (visibleOrInvisibleStack != null)
              // visibleOrInvisibleStack.Children.Add(groupTitleStack);
            frame.Content = mainStack;
            visibleOrInvisibleStack.Children.Add(frame);
           // secondStackRef.Children.Add(groupTitleStack);
            secondStackRef.Children.Add(visibleOrInvisibleStack);
           // visibleOrInvisibleStack.BackgroundColor = Color.Red;
		}


        public void ThirdPartLayoutDesign(List<JSSEMasterBehavior> listOfObjectsPerCategary, StackLayout secondStackRef, string titleLabel, int CategaryId)
        {


            StackLayout visibleOrInvisibleStack = new StackLayout();
            visibleOrInvisibleStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            visibleOrInvisibleStack.VerticalOptions = LayoutOptions.Start;
            //visibleOrInvisibleStack.BackgroundColor = Color.Red;
            visibleOrInvisibleStack.Orientation = StackOrientation.Vertical;
            visibleOrInvisibleStack.Spacing = 0;
            visibleOrInvisibleStack.Padding = new Thickness(10);
            visibleOrInvisibleStack.StyleId = "subbbbbbbb";

            Frame frame = new Frame();
            frame.Padding = new Thickness(0,10,0,0);
            frame.OutlineColor = Color.Black;
            frame.CornerRadius = 0;
            frame.HasShadow = false;
            frame.StyleId = "hiiiiii";
            //frame.IsVisible = false;
            // frame.Margin = new Thickness(10,0,10,0);

            StackLayout mainStack = new StackLayout();
            mainStack.Orientation = StackOrientation.Vertical;
            mainStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            mainStack.VerticalOptions = LayoutOptions.Start;
            mainStack.Spacing = 0;


            // StackLayout groupTitleStack = null;

            for (int i = 0; i < listOfObjectsPerCategary.Count; i++)
            {

                StackLayout groupTitleStack = new StackLayout();
                groupTitleStack.HorizontalOptions = LayoutOptions.FillAndExpand;
                groupTitleStack.VerticalOptions = LayoutOptions.Start;
                groupTitleStack.Padding = new Thickness(0);
                groupTitleStack.Spacing = 0;
                groupTitleStack.BackgroundColor = Color.Gray;
                groupTitleStack.Orientation = StackOrientation.Horizontal;

                StackLayout secondPartStack = new StackLayout();
                secondPartStack.HorizontalOptions = LayoutOptions.FillAndExpand;
                secondPartStack.VerticalOptions = LayoutOptions.Start;
                // secondPartStack.BackgroundColor = Color.Red;
                if (i == 0)
                {

                    Image arrowImage = new Image();
                    arrowImage.Source = "ArrowDown";
                    arrowImage.HorizontalOptions = LayoutOptions.End;
                    arrowImage.VerticalOptions = LayoutOptions.Center;
                    arrowImage.HeightRequest = 25;
                    arrowImage.WidthRequest = 25;

                    //adding heading 
                    Button headingLabel = new Button();
                    headingLabel.Margin = new Thickness(20, 0, 0, 0);
                    headingLabel.HorizontalOptions = LayoutOptions.FillAndExpand;
                    headingLabel.VerticalOptions = LayoutOptions.End;
                    headingLabel.Text = titleLabel;
                    headingLabel.TextColor = Color.White;
                    headingLabel.FontSize = 14;
                    headingLabel.FontAttributes = FontAttributes.Bold;
                    headingLabel.HeightRequest = 30;
                    groupTitleStack.Children.Add(headingLabel);
                    groupTitleStack.Children.Add(arrowImage);

                    headingLabel.Clicked += (sender, e) => {
                        //var oooo = visibleOrInvisibleStack.Children.Count;
                        Button buttonRef = sender as Button;
                        StackLayout selecteLayoutParent = buttonRef.Parent.Parent as StackLayout;
                        int childCount = selecteLayoutParent.Children.Count;

                        StackLayout arrowImageparentStack = buttonRef.Parent as StackLayout;

                        Frame SelectedFrame = selecteLayoutParent.Children[selecteLayoutParent.Children.Count - 2 ] as Frame;
                        if (SelectedFrame.IsVisible)
                        {
                            Image imageRef = arrowImageparentStack.Children[1] as Image;
                            imageRef.Source = "Arrowup";
                            SelectedFrame.IsVisible = false;
                        }
                        else
                        {
                            Image imageRef = arrowImageparentStack.Children[1] as Image;
                            imageRef.Source = "ArrowDown";
                            SelectedFrame.IsVisible = true;
                        }
                        // SelectedFrame.BackgroundColor = Color.Green;

                    };

                    //groupTitleStack.Padding = new Thickness(0, 10, 0, 0);
                }
                else
                {
                    //groupTitleStack.Padding = new Thickness(0, 10, 0, 0);
                }

                StackLayout titleAndEditBtnStack = new StackLayout();
                titleAndEditBtnStack.Padding = new Thickness(20, 0, 20, 0);
                titleAndEditBtnStack.Orientation = StackOrientation.Horizontal;
                titleAndEditBtnStack.HorizontalOptions = LayoutOptions.FillAndExpand;
                titleAndEditBtnStack.VerticalOptions = LayoutOptions.Start;
                titleAndEditBtnStack.Spacing = 5;

                Label label = new Label();
                label.HorizontalOptions = LayoutOptions.FillAndExpand;
                label.Text = listOfObjectsPerCategary[i].Behavior;
                label.FontSize = 14;

                Button editButton = new Button();
                editButton.Image = "edit";
                editButton.HorizontalOptions = LayoutOptions.End;
                editButton.WidthRequest = 20;
                editButton.HeightRequest = 20;
                editButton.StyleId = listOfObjectsPerCategary[i].Behavior;

                editButton.Clicked += (sender, e) => {


                    Button selectedLayout = sender as Button;
                    StackLayout selecteLayoutParent = selectedLayout.Parent.Parent as StackLayout;
                    var selectedObject = selecteLayoutParent.Children.Count;
                    var countVal = selecteLayoutParent.Children.Count;
                    // var obj = visibleOrInvisible.StyleId;
                    //StackLayout selectedStackChildern = selectedLayout.Parent.FindByName<StackLayout>("isVisibleOrInVisibleRef");

                    //listOfObjectsPerCategary = await App.Database.GetJsseBehaviors(0, 1);
                    if (selecteLayoutParent.Children.Count > 1)
                    {
                        StackLayout childStackLayout = selecteLayoutParent.Children[selecteLayoutParent.Children.Count - 1] as StackLayout;
                        if (childStackLayout.Children.Count == 4)
                        {

                            StackLayout editorStack = childStackLayout.Children[3] as StackLayout;
                            for (int j = 0; j < editorStack.Children.Count; j++)
                            {
                                SelectedEditor = editorStack.Children[0] as Editor;
                                var styleId = SelectedEditor.StyleId;
                                editorStack.IsVisible = true;
                                CommentsView.IsVisible = true;
                                titleRefLabel.Text = selectedLayout.StyleId;
                                CommentsEditorref.Text = SelectedEditor.Text;
                            }
                        }

                    }



                };

                //mainStack.Children.Add(groupTitleStack);
                visibleOrInvisibleStack.Children.Add(groupTitleStack);
                //secondPartStack.Children.Add(groupTitleStack);
                //frame.Content = groupTitleStack;
                titleAndEditBtnStack.Children.Add(label);
                titleAndEditBtnStack.Children.Add(editButton);
                secondPartStack.Children.Add(titleAndEditBtnStack);

                StackLayout ratingsLayout = new StackLayout();
                ratingsLayout.Orientation = StackOrientation.Vertical;
                ratingsLayout.HorizontalOptions = LayoutOptions.FillAndExpand;
                ratingsLayout.VerticalOptions = LayoutOptions.Start;
                ratingsLayout.Spacing = 5;

                SubRatingsDesignLayout(listOfRatings, ratingsLayout, listOfObjectsPerCategary[i].Behavior_ID, CategaryId, listOfObjectsPerCategary[i].BehaviorType_ID);

                secondPartStack.Children.Add(ratingsLayout);
                // visibleOrInvisibleStack.Children.Add(secondPartStack);
                mainStack.Children.Add(secondPartStack);
            }
            //if (visibleOrInvisibleStack != null)
            // visibleOrInvisibleStack.Children.Add(groupTitleStack);
            frame.Content = mainStack;
            visibleOrInvisibleStack.Children.Add(frame);

            takePhotoOrAlbumStack(visibleOrInvisibleStack, CategaryId);

            //visibleOrInvisibleStack.Children.Add(selectedLayoutsss);
            // secondStackRef.Children.Add(groupTitleStack);
            secondStackRef.Children.Add(visibleOrInvisibleStack);
        }

        public void takePhotoOrAlbumStack( StackLayout visibleOrInvisible, int CategaryId){
            
            StackLayout mainStack = new StackLayout();
            mainStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            mainStack.VerticalOptions = LayoutOptions.StartAndExpand;
            mainStack.Orientation = StackOrientation.Horizontal;
            mainStack.Padding = new Thickness(10);


            Image imageRef = new Image();
            imageRef.HeightRequest = 50;
            imageRef.WidthRequest = 150;
            imageRef.Source = "cameraIcon.png";

            Image imageRefTwo = new Image();
            imageRefTwo.HeightRequest = 50;
            imageRefTwo.WidthRequest = 150;
            imageRefTwo.Source = "albumIcon.png";

            //Frame photoFrame = new Frame();
            //photoFrame.Padding = new Thickness(10, 3, 10, 3);
            //photoFrame.HorizontalOptions = LayoutOptions.FillAndExpand;
            //photoFrame.HasShadow = false;
            //photoFrame.OutlineColor = Color.Blue;
            //photoFrame.CornerRadius = 0;
            ////photoFrame.BackgroundColor = Color.Purple;

            //StackLayout subStack = new StackLayout();
            //subStack.HorizontalOptions = LayoutOptions.CenterAndExpand;
            //subStack.VerticalOptions = LayoutOptions.StartAndExpand;
            //subStack.Spacing = 5;
            //subStack.Orientation = StackOrientation.Vertical;
            //subStack.BackgroundColor = Color.Purple;

            //Image imageRef = new Image();
            //imageRef.HeightRequest = 30;
            //imageRef.WidthRequest = 30;
            //imageRef.BackgroundColor = Color.Red;

            //Label labelRef = new Label();
            //labelRef.Text = "Camara";
            //labelRef.FontSize = 12;
            //labelRef.HorizontalOptions = LayoutOptions.CenterAndExpand;
            //labelRef.VerticalOptions = LayoutOptions.Start;

            //subStack.Children.Add(imageRef);
            //subStack.Children.Add(labelRef);

            //photoFrame.Content = subStack;


            //Frame albumFrame = new Frame();
            //albumFrame.Padding = new Thickness(10, 3, 10, 3);
            //albumFrame.HorizontalOptions = LayoutOptions.FillAndExpand;
            //albumFrame.HasShadow = false;
            //albumFrame.OutlineColor = Color.Blue;
            //albumFrame.CornerRadius = 0;
            //albumFrame.BackgroundColor = Color.Purple;

            //StackLayout subStackTwo = new StackLayout();
            //subStackTwo.HorizontalOptions = LayoutOptions.CenterAndExpand;
            //subStackTwo.VerticalOptions = LayoutOptions.StartAndExpand;
            //subStackTwo.Spacing = 5;
            //subStackTwo.Orientation = StackOrientation.Vertical;
            //subStackTwo.BackgroundColor = Color.Transparent;

            //Image imageRefTwo = new Image();
            //imageRefTwo.HeightRequest = 30;
            //imageRefTwo.WidthRequest = 30;
            //imageRefTwo.BackgroundColor = Color.Red;

            //Label labelRefTwo = new Label();
            //labelRefTwo.Text = "Album";
            //labelRefTwo.FontSize = 12;
            //labelRefTwo.HorizontalOptions = LayoutOptions.CenterAndExpand;
            //labelRefTwo.VerticalOptions = LayoutOptions.Start;

            //subStackTwo.Children.Add(imageRefTwo);
            //subStackTwo.Children.Add(labelRefTwo);

            //albumFrame.Content = subStackTwo;

            mainStack.Children.Add(imageRef);
            mainStack.Children.Add(imageRefTwo);

            visibleOrInvisible.Children.Add(mainStack);

            Button saveButtoninCategary = new Button();
            saveButtoninCategary.HorizontalOptions = LayoutOptions.FillAndExpand;
            saveButtoninCategary.VerticalOptions = LayoutOptions.Start;
            saveButtoninCategary.Text = "SAVE";
            saveButtoninCategary.BackgroundColor = Color.Gray;
            saveButtoninCategary.TextColor = Color.White;

            saveButtoninCategary.Clicked += (sender, e) => {
                
                Button selectedButton = sender as Button;
                StackLayout EditorStackParent = selectedButton.Parent as StackLayout;

                if (InputCategories.Count > 0)
                {
                    List<Categories> obj = InputCategories.Where(x => x.Category_ID == CategaryId).ToList();
                    if (obj.Count > 0)
                    {
                        if (obj[0].Comments == null || obj[0].Comments.Length == 0)
                        {
                            DisplayAlert("", "Please add comments", "OK");
                            return;
                        }
                    }
                    else
                    {
                        DisplayAlert("", "Please Select overall Rating", "OK");
                        return;
                    }
                }
                else
                {
                    DisplayAlert("", "Please Select overall Rating", "OK");
                    return;
                }



                StackLayout imageChangeStack = selectedButton.Parent.Parent.Parent as StackLayout;


                for (int b = 0; b < imageChangeStack.Children.Count; b++)
                {
                    int valiedValue = b * 3;
                    //if (b == 0 || b == 3 || b == 6 || b == 9 || b == 12 || b == 15)
                    if (b%3 == 0)
                    {
                        StackLayout ImageCheckStackParent = imageChangeStack.Children[b] as StackLayout;
                        Label labelOb = ImageCheckStackParent.Children[0] as Label;
                        if (selectedGridLabelName == labelOb.Text)
                        {
                            Image imageReffff = ImageCheckStackParent.Children[1] as Image;
                            imageReffff.BackgroundColor = Color.Green;
                        }
                        //else{
                        //    Image imageRef = ImageCheckStackParent.Children[1] as Image;
                        //    imageRef.BackgroundColor = Color.Red;
                        //}
                    }
                }


            };

            visibleOrInvisible.Children.Add(saveButtoninCategary);

        }

        public void OverAllRatingsDesign(List<RatingTable> listOfRatings, StackLayout layout, int mainId)
		{

			BoxView topLine = new BoxView();
			topLine.HorizontalOptions = LayoutOptions.FillAndExpand;
			topLine.VerticalOptions = LayoutOptions.Start;
			topLine.HeightRequest = 1;
			topLine.BackgroundColor = Color.Silver;
            layout.Children.Add(topLine);


			StackLayout ratingsLayoutSub = new StackLayout();
            ratingsLayoutSub.Orientation = StackOrientation.Horizontal;
			ratingsLayoutSub.HorizontalOptions = LayoutOptions.FillAndExpand;
			ratingsLayoutSub.VerticalOptions = LayoutOptions.Start;
			ratingsLayoutSub.Spacing = 0;
            ratingsLayoutSub.Padding = new Thickness(0);
			

            for (int i = 0; i < listOfRatings.Count; i++)
			{
				Button exceptionalBtn = new Button();
               // exceptionalBtn.Text = listOfRatings[i].Rating;
                if (listOfRatings[i].Rating == "Exceptional")
                {
                    exceptionalBtn.Image = "factorExceptionalInactive.png";
                }
                else if (listOfRatings[i].Rating == "Effective")
                {
                    exceptionalBtn.Image = "factorEffectiveInactive.png";
                }
                else
                {
                    exceptionalBtn.Image = "factorImpOppInactive.png";
                }
				exceptionalBtn.HorizontalOptions = LayoutOptions.FillAndExpand;
				exceptionalBtn.BackgroundColor = Color.Transparent;
                exceptionalBtn.StyleId = listOfRatings[i].Rating_ID.ToString();
				ratingsLayoutSub.Children.Add(exceptionalBtn);

                exceptionalBtn.Clicked += (sender, e) => {

                    Categories categoriesObj = new Categories();
                    Button selectedButton = sender as Button;

                    int cou = ratingsLayoutSub.Children.Count;
                    StackLayout EditorStackParent = selectedButton.Parent.Parent as StackLayout;

                    //StackLayout imageChangeStack = selectedButton.Parent.Parent.Parent.Parent.Parent as StackLayout;



                    //for (int b = 0; b < imageChangeStack.Children.Count; b++){
                    //    if (b==0 || b ==3 || b == 6 || b == 9 || b == 12 || b == 15){
                    //        StackLayout ImageCheckStackParent = imageChangeStack.Children[b] as StackLayout;
                    //        Label labelOb = ImageCheckStackParent.Children[0] as Label;
                    //        if (selectedGridLabelName == labelOb.Text)
                    //        {
                    //            Image imageRef = ImageCheckStackParent.Children[1] as Image;
                    //            imageRef.BackgroundColor = Color.Green;
                    //        }
                    //        //else{
                    //        //    Image imageRef = ImageCheckStackParent.Children[1] as Image;
                    //        //    imageRef.BackgroundColor = Color.Red;
                    //        //}
                    //    }
                    //}

                    if (EditorStackParent.Children.Count == 4){
                        StackLayout editorRefStack = EditorStackParent.Children[3] as StackLayout;
                        Editor editorRef = editorRefStack.Children[0] as Editor;
                        categoriesObj.Comments = editorRef.Text;
                    }

                    for (int m = 0; m < ratingsLayoutSub.Children.Count; m++){
						Button buttenRef = ratingsLayoutSub.Children[m] as Button;
                        if(buttenRef != null){
                            buttenRef.BackgroundColor = Color.Transparent;

                            if (m == 0)
                            {
                                buttenRef.Image = "factorExceptionalInactive.png";
                            }
                            else if (m == 2)
                            {
                                buttenRef.Image = "factorEffectiveInactive.png";
                            }
                            else if (m == 4)
                            {
                                buttenRef.Image = "factorImpOppInactive.png";
                            }
                        }
					}
                    //selectedButton.BackgroundColor = Color.Red;
                    if (selectedButton.StyleId == "1"){
                        selectedButton.Image = "factorExceptionalActive.png";
                    }else if (selectedButton.StyleId == "2"){
                        selectedButton.Image = "factorEffectiveActive.png";
                    }else{
                        selectedButton.Image = "factorImpOppActive.png";
                    }
                    string selectedratingId = selectedButton.StyleId;
                    int selectedcategaryId = mainId;


                    categoriesObj.Category_ID = mainId;
                    categoriesObj.RatingID = selectedratingId;
                    // categoriesObj.Comments = "";

                    //homeListView.ItemsSource = SortedResponceData.OrderBy(x => x.Name);
                    //List<Tanks> SortedResponceDataOne = (List<Tanks>)SortedResponceData.Where(x => x.GallonVolume == "1144");
                    List<Categories> SortedResponceDataOne = InputCategories.Where(x => x.Category_ID == mainId).ToList();
                    if (SortedResponceDataOne.Count == 0)
                    {
                        InputCategories.Add(categoriesObj);
                    }else{
                        for (int h = 0; h < InputCategories.Count; h++)
                        {
                            Categories obj = InputCategories[h];
                            if(obj.Category_ID == mainId){

                                InputCategories[h].Category_ID = mainId;
                                InputCategories[h].Comments = categoriesObj.Comments;
                                InputCategories[h].RatingID = selectedratingId;
                            }
                        }
                    }

                    saveBtnRef.IsEnabled = true;

                };
				

                if (i != listOfRatings.Count - 1)
				{
					BoxView verticalLine = new BoxView();
					verticalLine.HorizontalOptions = LayoutOptions.Start;
					verticalLine.VerticalOptions = LayoutOptions.FillAndExpand;
					verticalLine.WidthRequest = 1;
					verticalLine.BackgroundColor = Color.Silver;
					ratingsLayoutSub.Children.Add(verticalLine);
				}
			}

            layout.Children.Add(ratingsLayoutSub);

			BoxView bottomLine = new BoxView();
			bottomLine.HorizontalOptions = LayoutOptions.FillAndExpand;
			bottomLine.VerticalOptions = LayoutOptions.Start;
			bottomLine.HeightRequest = 1;
			bottomLine.BackgroundColor = Color.Silver;
			layout.Children.Add(bottomLine);

            StackLayout editorStack = new StackLayout();
            editorStack.Padding = new Thickness(20, 0, 20, 0);
            editorStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            editorStack.VerticalOptions = LayoutOptions.Start;
            editorStack.IsVisible = false;
            editorStack.Orientation = StackOrientation.Horizontal;

            Editor editor = new Editor();
            editor.HorizontalOptions = LayoutOptions.FillAndExpand;
            editor.VerticalOptions = LayoutOptions.StartAndExpand;
            editor.FontSize = 12;
            editor.HeightRequest = 20;
            editor.WidthRequest = 240;
            editor.IsEnabled = false;
            editor.StyleId = mainId.ToString();


            Button moreButton = new Button();
            moreButton.Text = "More";
            moreButton.HorizontalOptions = LayoutOptions.End;
            moreButton.WidthRequest = 60;
            moreButton.Clicked += (sender, e) => {

                Button buttonref = sender as Button;
                StackLayout parentStack = buttonref.Parent as StackLayout;
                if(parentStack.Children.Count >1){

                    Editor editorref = parentStack.Children[0] as Editor;
                    if (buttonref.Text == "More")
                    {
                        editorref.HeightRequest = 60;
                        buttonref.Text = "Less";
                    }else{
                        editorref.HeightRequest = 20;
                        buttonref.Text = "More";
                    }
                }
            };

            editorStack.Children.Add(editor);
            editorStack.Children.Add(moreButton);
            layout.Children.Add(editorStack);


		}



        public void SubRatingsDesignLayout(List<RatingTable> listOfRatings, StackLayout layout, int BehaviourId, int CategaryId, int BehaviourTypeId)
        {

            BoxView topLine = new BoxView();
            topLine.HorizontalOptions = LayoutOptions.FillAndExpand;
            topLine.VerticalOptions = LayoutOptions.Start;
            topLine.HeightRequest = 1;
            topLine.BackgroundColor = Color.Silver;
            layout.Children.Add(topLine);


            StackLayout ratingsLayoutSub = new StackLayout();
            ratingsLayoutSub.Orientation = StackOrientation.Horizontal;
            ratingsLayoutSub.HorizontalOptions = LayoutOptions.FillAndExpand;
            ratingsLayoutSub.VerticalOptions = LayoutOptions.Start;
            ratingsLayoutSub.Spacing = 0;
            ratingsLayoutSub.Padding = new Thickness(0);


            for (int i = 0; i < listOfRatings.Count; i++)
            {
                Button exceptionalBtn = new Button();
               // exceptionalBtn.Text = listOfRatings[i].Rating;
                if (listOfRatings[i].Rating == "Exceptional"){
                    exceptionalBtn.Image = "factorExceptionalInactive.png";
                }else if (listOfRatings[i].Rating == "Effective"){
                    exceptionalBtn.Image = "factorEffectiveInactive.png";
                }else{
                    exceptionalBtn.Image = "factorImpOppInactive.png";
                }
                exceptionalBtn.HorizontalOptions = LayoutOptions.FillAndExpand;
                exceptionalBtn.BackgroundColor = Color.Transparent;
                exceptionalBtn.StyleId = listOfRatings[i].Rating_ID.ToString();
                ratingsLayoutSub.Children.Add(exceptionalBtn);

                exceptionalBtn.Clicked += (sender, e) => {
                    EntBehavior entBehavior = new EntBehavior();
                    Button selectedButton = sender as Button;

                    int cou = ratingsLayoutSub.Children.Count;

                    StackLayout EditorStackParent = selectedButton.Parent.Parent as StackLayout;

                    if (EditorStackParent.Children.Count == 4)
                    {
                        StackLayout editorRefStack = EditorStackParent.Children[3] as StackLayout;
                        Editor editorRef = editorRefStack.Children[0] as Editor;
                        entBehavior.Comments = editorRef.Text;
                    }

                    for (int m = 0; m < ratingsLayoutSub.Children.Count; m++)
                    {
                        Button buttenRef = ratingsLayoutSub.Children[m] as Button;
                        if (buttenRef != null)
                        {
                            buttenRef.BackgroundColor = Color.Transparent;
                            if (m == 0)
                            {
                                buttenRef.Image = "factorExceptionalInactive.png";
                            }
                            else if (m == 2)
                            {
                                buttenRef.Image = "factorEffectiveInactive.png";
                            }
                            else if(m == 4)
                            {
                                buttenRef.Image = "factorImpOppInactive.png";
                            }
                        }
                    }


                    ////////////////// Validations//////////TODO
                    //if (InputCategories.Count > 0)
                    //{
                    //    List<Categories> obj = InputCategories.Where(x => x.Category_ID == CategaryId).ToList();
                    //    if (obj.Count > 0)
                    //    {
                    //        if (obj[0].Comments == null || obj[0].Comments.Length == 0)
                    //        {
                    //            DisplayAlert("", "Please add comments", "OK");
                    //            return;
                    //        }
                    //    }else{
                    //        DisplayAlert("", "Please Select overall Rating", "OK");
                    //        return;
                    //    }
                    //}
                    //else
                    //{
                    //    DisplayAlert("", "Please Select overall Rating", "OK");
                    //    return;
                    //}

                    /// ///////////////////////////////


                   // selectedButton.BackgroundColor = Color.Red;
                     if (selectedButton.StyleId == "1")
                    {
                        selectedButton.Image = "factorExceptionalActive.png";
                    }
                    else if (selectedButton.StyleId == "2")
                    {
                        selectedButton.Image = "factorEffectiveActive.png";
                    }
                    else
                    {
                        selectedButton.Image = "factorImpOppActive.png";
                    }
                    string selectedratingId = selectedButton.StyleId;
                    int selectedBehaviourId = BehaviourId;
                    int selectedCategaryId = CategaryId;

                   
                    entBehavior.Category_ID = CategaryId;
                    entBehavior.Rating_ID = Convert.ToInt32(selectedratingId);
                    entBehavior.Behavior_ID = selectedBehaviourId;
                    entBehavior.BehaviorType_ID = BehaviourTypeId;

                   

                    List<EntBehavior> SortedResponceDataOne = InputOrgBehaviors.Where(x => x.Category_ID == CategaryId && x.Behavior_ID == selectedBehaviourId ).ToList();
					if (SortedResponceDataOne.Count == 0)
					{
						InputOrgBehaviors.Add(entBehavior);
					}
					else
					{
						for (int h = 0; h < InputOrgBehaviors.Count; h++)
						{
							EntBehavior obj = InputOrgBehaviors[h];
                            if (obj.Category_ID == CategaryId && obj.Behavior_ID == selectedBehaviourId)
							{

                                InputOrgBehaviors[h].Category_ID = CategaryId;
								InputOrgBehaviors[h].Comments = entBehavior.Comments;
                                InputOrgBehaviors[h].Behavior_ID = selectedBehaviourId;
                                InputOrgBehaviors[h].Rating_ID = Convert.ToInt32(selectedratingId);
                                InputOrgBehaviors[h].BehaviorType_ID = entBehavior.BehaviorType_ID;
							}
						}
					}

                    //InputOrgBehaviors.Add(entBehavior);
                    saveBtnRef.IsEnabled = true;
                      

                };


                if (i != listOfRatings.Count - 1)
                {
                    BoxView verticalLine = new BoxView();
                    verticalLine.HorizontalOptions = LayoutOptions.Start;
                    verticalLine.VerticalOptions = LayoutOptions.FillAndExpand;
                    verticalLine.WidthRequest = 1;
                    verticalLine.BackgroundColor = Color.Silver;
                    ratingsLayoutSub.Children.Add(verticalLine);
                }
            }

            layout.Children.Add(ratingsLayoutSub);

            BoxView bottomLine = new BoxView();
            bottomLine.HorizontalOptions = LayoutOptions.FillAndExpand;
            bottomLine.VerticalOptions = LayoutOptions.Start;
            bottomLine.HeightRequest = 1;
            bottomLine.BackgroundColor = Color.Silver;
            layout.Children.Add(bottomLine);

            StackLayout editorStack = new StackLayout();
            editorStack.Padding = new Thickness(20, 0, 20, 0);
            editorStack.HorizontalOptions = LayoutOptions.FillAndExpand;
            editorStack.VerticalOptions = LayoutOptions.Start;
            editorStack.IsVisible = false;
            editorStack.Orientation = StackOrientation.Horizontal;

            Editor editor = new Editor();
            editor.HorizontalOptions = LayoutOptions.FillAndExpand;
            editor.VerticalOptions = LayoutOptions.StartAndExpand;
            editor.FontSize = 12;
            editor.HeightRequest = 20;
            editor.WidthRequest = 280;
            editor.IsEnabled = false;
            editor.StyleId = CategaryId.ToString()+ " " + BehaviourId;


            Button moreButton = new Button();
            moreButton.Text = "More";
            moreButton.HorizontalOptions = LayoutOptions.End;
            moreButton.WidthRequest = 60;
            moreButton.Clicked += (sender, e) => {

                Button buttonref = sender as Button;
                StackLayout parentStack = buttonref.Parent as StackLayout;
                if (parentStack.Children.Count > 1)
                {

                    Editor editorref = parentStack.Children[0] as Editor;
                    if (buttonref.Text == "More")
                    {
                        editorref.HeightRequest = 60;
                        buttonref.Text = "Less";
                    }
                    else
                    {
                        editorref.HeightRequest = 20;
                        buttonref.Text = "More";
                    }
                }
            };

            editorStack.Children.Add(editor);
            editorStack.Children.Add(moreButton);
            layout.Children.Add(editorStack);


        }

		public Task<int> ReadDataFromJson()
		{
			var assembly = typeof(DynamicScreen).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("SampleDB.getbannerdata.json");

			using (var reader = new System.IO.StreamReader(stream))
			{

				var json = reader.ReadToEnd();
				List<GetActiveCategoriesModel.RootObject> data = JsonConvert.DeserializeObject<List<GetActiveCategoriesModel.RootObject>>(json);

				GetActiveCategoriesModel.RootObject[] arrayobj = data.ToArray();
				for (int k = 0; k < arrayobj.Length; k++)
				{
					JSSEMasterCategory mctbl = new JSSEMasterCategory();
					mctbl.Category_ID = arrayobj[k].Category_ID;
					mctbl.Category = arrayobj[k].Category;
					App.Database.SaveCategoriesAsync(mctbl);
					for (int t = 0; t < arrayobj[k].Ratings.Count; t++)
					{
						RatingTable rtbl = new RatingTable();
						rtbl.Rating_ID = arrayobj[k].Ratings[t].Rating_ID;
						rtbl.Rating = arrayobj[k].Ratings[t].Rating;
						App.Database.SaveRatingsAsync(rtbl);

					}
					GetActiveCategoriesModel.EntBehavior[] eorgdata = arrayobj[k].EntBehaviors.ToArray();
					for (int l = 0; l < eorgdata.Length; l++)
					{
						JSSEMasterBehavior mbhtbl = new JSSEMasterBehavior();

						mbhtbl.Behavior_ID = eorgdata[l].Behavior_ID;
						mbhtbl.Behavior = eorgdata[l].Behavior;
						mbhtbl.Category_ID = arrayobj[k].Category_ID;
						mbhtbl.BehaviorType_ID = eorgdata[l].BehaviorType_ID;
						App.Database.SaveBehaviorssAsync(mbhtbl);
					}

					for (int i = 0; i < arrayobj[k].AllOrgBehaviors.Count; i++)
					{
						for (int j = 0; j < arrayobj[k].AllOrgBehaviors[i].Count; j++)
						{
							JSSEMasterBehavior mbhtbl = new JSSEMasterBehavior();

							mbhtbl.Behavior_ID = arrayobj[k].AllOrgBehaviors[i][j].Behavior_ID;
							mbhtbl.Behavior = arrayobj[k].AllOrgBehaviors[i][j].Behavior;
							mbhtbl.Category_ID = arrayobj[k].Category_ID;
							mbhtbl.Org_ID = arrayobj[k].AllOrgBehaviors[i][j].Org_ID;
							mbhtbl.BehaviorType_ID = arrayobj[k].AllOrgBehaviors[i][j].BehaviorType_ID;
							App.Database.SaveBehaviorssAsync(mbhtbl);
						}
					}
				}



			}

			return Task.FromResult(1);
		}

		void SaveBtnRef_Clicked(object sender, EventArgs e)
		{
            Debug.WriteLine(InputCategories.Count);
            Debug.WriteLine(InputOrgBehaviors.Count);
		}

    }
}
