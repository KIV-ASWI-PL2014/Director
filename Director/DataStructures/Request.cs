using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Director.DataStructures.Exceptions;
using System.IO;
using Director.DataStructures.SupportStructures;
using Xwt;
using RestSharp;
using Director.Formatters;
using Xwt.Drawing;
using Director.Forms.Controls;
using System.Xml;
using System.ComponentModel;

namespace Director.DataStructures
{
    /// <summary>
    /// Request representation class.
    /// </summary>
    [Serializable]
    public class Request
    {
        /// <summary>
        /// Parent scenario (non serialized - cycles)
        /// </summary>
        /// <value>The parent scenario.</value>
        [XmlIgnore]
        public Scenario ParentScenario { get; set; }

        /// <summary>
        /// Tree position - set when creating.
        /// </summary>
        [XmlIgnore]
        public TreePosition TreePosition { get; set; }

        /// <summary>
        /// Request ID in scenario list.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Request position - working position.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Request name.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Requests url address.
        /// </summary>
        /// <value>The URL.</value>
        public String Url { get; set; }

        /// <summary>
        /// HTTP headers.
        /// </summary>
        public List<Header> Headers = new List<Header>();

        /// <summary>
        /// Files.
        /// </summary>
        public List<FileItem> Files { get; set; }

        /// <summary>
        /// Expected response code.
        /// </summary>
        public int ExpectedStatusCode { get; set; }

        /// <summary>
        /// Request Method (POST|GET|PUT)
        /// </summary>
        public String HTTP_METHOD { get; set; }

        /// <summary>
        /// Request String template.
        /// </summary>
        [XmlIgnore]
        public String RequestTemplate { get; set; }

		/// <summary>
		/// Request template types.
		/// </summary>
		/// <value>The type of the request template.</value>
		[XmlIgnore]
		public ContentType RequestTemplateType { get; set; }

        /// <summary>
        /// Request repeat count!
        /// </summary>
        [DefaultValue(1)]
        public int RepeatsCounter { get; set; }

        /// <summary>
        /// Request repeat time between repeats in seconds.
        /// </summary>
        [DefaultValue(1)]
        public int RepeatsTimeout { get; set; }

		/// <summary>
		/// Request template types.
		/// </summary>
		/// <value>The type of the request template.</value>
		[XmlElement("RequestTemplateType")]
		public String RequestTemplateTypeS { 
			get {
				return ContentTypeUtils.ToString (RequestTemplateType);
			}
			set {
				RequestTemplateType = ContentTypeUtils.FromString (value);
			}
		}

        /// <summary>
        /// Request cdata seriailization.
        /// </summary>
        [XmlElement("RequestTemplate")]
        public XmlCDataSection RequestTemplateCD
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(RequestTemplate);
            }
            set
            {
                RequestTemplate = value.Value;
            }
        }

        /// <summary>
        /// Response string template.
        /// </summary>
        [XmlIgnore]
        public String ResponseTemplate { get; set; }


		/// <summary>
		/// Response template.
		/// </summary>
		/// <value>The response template.</value>
		[XmlIgnore]
		public ContentType ResponseTemplateType { get; set; }
				

		/// <summary>
		/// Request template types.
		/// </summary>
		/// <value>The type of the request template.</value>
		[XmlElement("ResponseTemplateType")]
		public String ResponseTemplateTypeS { 
			get {
				return ContentTypeUtils.ToString (ResponseTemplateType);
			}
			set {
				ResponseTemplateType = ContentTypeUtils.FromString (value);
			}
		}

        /// <summary>
        /// Response CData serialization.
        /// </summary>
        [XmlElement("ResponseTemplate")]
        public XmlCDataSection ResponseTemplateCD
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                return doc.CreateCDataSection(ResponseTemplate);
            }
            set
            {
                ResponseTemplate = value.Value;
            }
        }

        /// <summary>
        /// Wait after previous request time! (in seconds)
        /// </summary>
        public int WaitAfterPreviousRequest { get; set; }

        /// <summary>
        /// Request remote result.
        /// </summary>
        [XmlIgnore]
        public List<ResultViewItem> RequestRemoteResult { get; set; }

        /// <summary>
        /// Result view items
        /// </summary>
        public struct ResultViewItem
        {
            public int Type; // 1 title, 2 item, 3 text edit
            public String Data;
        }

        /// <summary>
        /// Clear result items.
        /// </summary>
        internal void ClearResults()
        {
            if (RequestRemoteResult == null)
                RequestRemoteResult = new List<ResultViewItem>();
            else
                RequestRemoteResult.Clear();
        }

        /// <summary>
        /// Add result item to view.
        /// </summary>
        public void AddResultViewItem(int type, String ret)
        {
            RequestRemoteResult.Add(new ResultViewItem()
            {
                Type = type, Data = ret
            });
        }

        /// <summary>
        /// Create result vbox data item.
        /// </summary>
        public void CreateResult(VBox RequestStatus, Font CaptionFont)
        {
            // Clear
            RequestStatus.Clear();

            // Nothing to do!
            if (RequestRemoteResult == null)
            {
                RequestStatus.PackStart(new Label(Director.Properties.Resources.NoResponse)
                {
                    Font = CaptionFont
                });
                return;
            }

            // iterate
            bool first = true;
            foreach (var i in RequestRemoteResult)
            {
                if (i.Type == 1)
                {
                    RequestStatus.PackStart(new Label(i.Data)
                    {
                        Font = CaptionFont,
                        MarginTop = (first) ? 0 : 20
                    }, false, false);
                }
                else if (i.Type == 2)
                {
                    RequestStatus.PackStart(new ListItem(i.Data));
                }
                else
                {
                    MultiLineTextEntry RequestTextEntry = new MultiLineTextEntry()
                    {
                        Margin = 10,
						Text = i.Data,
						Sensitive = false
                    };
                    RequestStatus.PackStart(RequestTextEntry);
                    Button ClipboardButtonReq = new Button(Image.FromResource(DirectorImages.COPY_ICON), "")
                    {
                        WidthRequest = 30,
                        HeightRequest = 30,
                        ExpandHorizontal = false,
                        ExpandVertical = false,
                        MarginRight = 10,
                        TooltipText = Director.Properties.Resources.CopyInClipboard
                    };
                    ClipboardButtonReq.Clicked += delegate
                    {
                        Clipboard.SetText(RequestTextEntry.Text);
                    };
                    RequestStatus.PackStart(ClipboardButtonReq, hpos: WidgetPlacement.End);
                }

                first = false;
            }
        }

        /// <summary>
        /// Empty constructor for XML serialization!
        /// </summary>
        public Request()
            : this(0, 0, "")
        {
        }

        /// <summary>
        /// Parametrize contructor for manually creating request.
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="position">Position</param>
        /// <param name="name">Request identifier</param>
        public Request(int id, int position, String name)
        {
            Id = id;
            Position = position;
            Name = name;
            HTTP_METHOD = "GET";
            Files = new List<FileItem>();
        }

        /// <summary>
        /// Url settings
        /// </summary>
        /// <param name="url"></param>
        public void SetUrl(String url)
        {
            Uri _newUri;

            // Uri validation
            if (Uri.TryCreate(url, UriKind.Absolute, out _newUri))
                this.Url = _newUri.AbsoluteUri;
            else
                throw new InvalidUrlException();
        }

        /// <summary>
        /// Get clone of myself.
        /// </summary>
        /// <returns></returns>
        public Request Clone()
        {
            XmlSerializer ser = new XmlSerializer(typeof(Request));
            StringBuilder sb = new StringBuilder();
            TextWriter writer = new StringWriter(sb);
            ser.Serialize(writer, this);
            TextReader reader = new StringReader(sb.ToString());
            Request t = (Request)ser.Deserialize(reader);
            t.ParentScenario = this.ParentScenario;

            // SEt new position
            t.Position = this.ParentScenario.Requests.Max(n => n.Position) + 1;

            // Tree position is set by GUI
            return t;
        }

        /// <summary>
        /// Create overview!
        /// </summary>
        public void CreateOverview(VBox RequestOverview, Font CaptionFont)
        {
            // Overview clear
            RequestOverview.Clear();

            // Information
            RequestOverview.PackStart(new Label(Director.Properties.Resources.RequestUrl + ":")
            {
                Font = CaptionFont

            }, false, false);

            // Create URL
            RequestOverview.PackStart(new LinkLabel(Url)
            {
                Uri = new Uri(Url),
                MarginLeft = 10
            }, false, false);

            // Method
            RequestOverview.PackStart(new Label(Director.Properties.Resources.RequestMethod + ":")
            {
                Font = CaptionFont,
                MarginTop = 20
            }, false, false);

            // Create URL
            RequestOverview.PackStart(new Label(HTTP_METHOD)
            {
                MarginLeft = 10
            }, false, false);

            // Headers
            if (Headers.Count > 0)
            {
                RequestOverview.PackStart(new Label(Director.Properties.Resources.RequestHeaders + ":")
                {
                    Font = CaptionFont,
                    MarginTop = 20
                }, false, false);
                foreach (var h in Headers)
                    RequestOverview.PackStart(new ListItem(string.Format("{0} - {1}", h.Name, h.Value)));
            }

            // Files
            if (Files.Count > 0)
            {
                RequestOverview.PackStart(new Label(Director.Properties.Resources.RequestFiles + ":")
                {
                    Font = CaptionFont,
                    MarginTop = 20
                }, false, false);
                foreach (var h in Files)
                    RequestOverview.PackStart(new ListItem(h.FileName));
            }

            // Request
            // Request
            if (RequestTemplate != null && RequestTemplate.Length > 0)
            {
				RequestOverview.PackStart(new Label(string.Format("{0} ({1}) :", Director.Properties.Resources.RequestTemplate, RequestTemplateTypeS))
                {
                    Font = CaptionFont,
                    MarginTop = 20
                }, false, false);

                MultiLineTextEntry RequestTextEntry = new MultiLineTextEntry()
                {
					Margin = 10, Sensitive = false
                };
						
				if (RequestTemplateType == ContentType.JSON)
                {
                    RequestTextEntry.Text = JSONFormatter.Format(RequestTemplate);
                    
                    if (RequestTextEntry.Text == null || RequestTextEntry.Text.Trim().Length == 0)
                        RequestTextEntry.Text = RequestTemplate;
                }
                else
                {
                    RequestTextEntry.Text = RequestTemplate;
                }
                RequestOverview.PackStart(RequestTextEntry);


                Button ClipboardButtonReq = new Button(Image.FromResource(DirectorImages.COPY_ICON), "")
                {
                    WidthRequest = 30,
                    HeightRequest = 30,
                    ExpandHorizontal = false,
                    ExpandVertical = false,
                    MarginRight = 10,
                    TooltipText = Director.Properties.Resources.CopyInClipboard
                };
                ClipboardButtonReq.Clicked += delegate
                {
                    Clipboard.SetText(RequestTextEntry.Text);
                };
                RequestOverview.PackStart(ClipboardButtonReq, hpos: WidgetPlacement.End);
            }

            // Requested code
            if (ExpectedStatusCode > 0)
            {
                RequestOverview.PackStart(new Label(Director.Properties.Resources.ExpectedStatusCode + ":")
                {
                    Font = CaptionFont
                }, false, false);
                RequestOverview.PackStart(new ListItem(ExpectedStatusCode + ""));
            }


            // Request
            if (ResponseTemplate != null && ResponseTemplate.Length > 0)
            {
				RequestOverview.PackStart(new Label(string.Format("{0} ({1}): ", Director.Properties.Resources.ResponseTemplate, ResponseTemplateTypeS))
                {
                    Font = CaptionFont,
                    MarginTop = (ExpectedStatusCode > 0) ? 20 : 0
                }, false, false);

                MultiLineTextEntry ResponseTextEntry = new MultiLineTextEntry()
                {
					Margin = 10, Sensitive = false
                };

                if (ResponseTemplate.Trim().StartsWith("{"))
                {
                    ResponseTextEntry.Text = JSONFormatter.Format(ResponseTemplate);
                }
                else
                {
                    ResponseTextEntry.Text = ResponseTemplate;
                }
                RequestOverview.PackStart(ResponseTextEntry);
                Button ClipboardButton = new Button(Image.FromResource(DirectorImages.COPY_ICON), "")
                {
                    WidthRequest = 30,
                    HeightRequest = 30,
                    ExpandHorizontal = false,
                    ExpandVertical = false,
                    MarginRight = 10,
                    MarginBottom = 10,
                    TooltipText = Director.Properties.Resources.CopyInClipboard
                };
                ClipboardButton.Clicked += delegate
                {
                    Clipboard.SetText(ResponseTextEntry.Text);
                };
                RequestOverview.PackStart(ClipboardButton, hpos: WidgetPlacement.End);
            }
        }

        /// <summary>
        /// Request method POST.
        /// </summary>
        public Method RequestMethod
        {
            get
            {
                switch (HTTP_METHOD.ToLower())
                {
                    case "post":
                        return Method.POST;

                    case "delete":
                        return Method.DELETE;

                    case "head":
                        return Method.HEAD;

                    case "options":
                        return Method.OPTIONS;

                    case "patch":
                        return Method.PATCH;

                    case "put":
                        return Method.PUT;

                    default:
                        return Method.GET;
                }
            }
        }
    }
}