using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Browser;

namespace SharpGIS.MouseExtensions
{
	public class RightMouseButtonEventArgs : EventArgs
	{
		Point Point;
		internal RightMouseButtonEventArgs(UIElement sender, Point point)
		{
			OriginalSource = sender;
			this.Point = point;
		}
		public Point GetPosition(UIElement relativeTo)
		{
			return OriginalSource.TransformToVisual(relativeTo).Transform(Point);
		}
		public UIElement OriginalSource { get; internal set; }
		public bool Handled { get; set; }
	}

	public static class RightClick
	{
		private static RightClickWorker worker;
		
		private static readonly DependencyProperty RightClickHandlersProperty = DependencyProperty.RegisterAttached("RightClickHandlers", typeof(IList<EventHandler<RightMouseButtonEventArgs>>), typeof(RightClick), null);
		private static UIElement currentElement;

		public static void AttachRightClick(this UIElement element, EventHandler<RightMouseButtonEventArgs> handler)
		{
			if (worker == null)
			{
				worker = new RightClickWorker();
				worker.RightClick += worker_RightClick;
			}

			element.MouseEnter += HandleMouseEnter; //Enable
			//element.MouseMove += HandleMouseEnter;  //Enable
			element.MouseLeave += HandleMouseLeave; //Disable

			IList<EventHandler<RightMouseButtonEventArgs>> handlers = element.GetValue(RightClickHandlersProperty) as IList<EventHandler<RightMouseButtonEventArgs>>;
			if (handlers == null)
			{
				handlers = new List<EventHandler<RightMouseButtonEventArgs>>();
				element.SetValue(RightClickHandlersProperty, handlers);
			}
			handlers.Add(handler);
		}

		public static void DetachRightClick(this UIElement element, EventHandler<RightMouseButtonEventArgs> handler)
		{
			element.MouseEnter -= HandleMouseEnter; //Enable
			//element.MouseMove -= HandleMouseEnter;  //Enable
			element.MouseLeave -= HandleMouseLeave; //Disable

			IList<EventHandler<RightMouseButtonEventArgs>> handlers = element.GetValue(RightClickHandlersProperty) as IList<EventHandler<RightMouseButtonEventArgs>>;
			if (handlers != null)
			{
				if (handlers.Contains(handler))
				{
					handlers.Remove(handler);
					if (handlers.Count == 0)
						element.ClearValue(RightClickHandlersProperty);
				}
			}
		}

		private static void HandleMouseEnter(object sender, EventArgs e)
		{
			currentElement = (sender as UIElement);
			worker.IsEnabled = true;
		}

		private static void HandleMouseLeave(object sender, EventArgs e)
		{
			currentElement = null;
			worker.IsEnabled = false;
		}

		private static void worker_RightClick(object sender, HtmlEventArgs e)
		{
			if (currentElement == null) return;
			IList<EventHandler<RightMouseButtonEventArgs>> handlers = currentElement.GetValue(RightClickHandlersProperty) as IList<EventHandler<RightMouseButtonEventArgs>>;
			if (handlers == null || handlers.Count == 0) return;
			Point p = new System.Windows.Point(e.OffsetX, e.OffsetY);
			p = System.Windows.Application.Current.RootVisual.TransformToVisual(currentElement).Transform(p);
			RightMouseButtonEventArgs args = new RightMouseButtonEventArgs(currentElement, p);			
			foreach (EventHandler<RightMouseButtonEventArgs> handler in handlers)
			{
				handler(currentElement, args);
			}
			if (args.Handled)
			{
				//Prevent Silverlight Plugin from receiving this event (buggy for FireFox)
				e.PreventDefault();
				e.StopPropagation();
			}
		}

		/// <summary>
		/// Uses the HTML Dom bridge for detecting right clicks.
		/// </summary>
		private class RightClickWorker
		{
			private bool IsInternetExplorer;
			public event EventHandler<HtmlEventArgs> RightClick;
			private bool isEnabled;
			HtmlElement glassDiv;

			public bool IsEnabled
			{
				get { return isEnabled; }
				set { 
					isEnabled = value;
					if (!value) HideGlass();
				}
			}
			/// <summary>
			/// Initializes a new instance of the <see cref="RightClickWorker"/> class.
			/// </summary>
			public RightClickWorker()
			{
				if (HtmlPage.IsEnabled && Application.Current.Host.Settings.Windowless)
				{
					IsInternetExplorer = HtmlPage.BrowserInformation.UserAgent.IndexOf("MSIE") > -1;
					if (IsInternetExplorer)
					{
						HtmlPage.Document.AttachEvent("oncontextmenu", this.HandleIEContextMenu);
					}
					else
					{
						HtmlPage.Document.AttachEvent("mousedown", this.HandleGlassMouseDown);
					}
				}
			}

			/// <summary>
			/// Creates a div on top of the plugin to prevent mouse events from
			/// firing on the plugin (Mozilla browsers only).
			/// </summary>
			private void ShowGlass()
			{
				if (glassDiv == null)
				{
					glassDiv = HtmlPage.Document.CreateElement("div");
					glassDiv.SetAttribute("style", "width:100%;height:100%;position:absolute;left:0;top:0;zIndex:100");
					glassDiv.AttachEvent("mouseup", this.HandleGlassMouseUp);
					glassDiv.AttachEvent("mousemove", (object s, HtmlEventArgs e) => { HideGlass(); });
					HtmlPage.Document.Body.AppendChild(glassDiv);
				}
			}

			private void HideGlass()
			{
				if (glassDiv != null)
				{
					HtmlPage.Document.Body.RemoveChild(glassDiv);
					glassDiv = null;
				}
			}

			private void HandleGlassMouseDown(object sender, HtmlEventArgs args)
			{
				if (IsEnabled && args.MouseButton == MouseButtons.Right)
				{
					ShowGlass();
				}
			}

			private void HandleGlassMouseUp(object sender, HtmlEventArgs args)
			{
				if (args.MouseButton == MouseButtons.Right)
				{
					OnRightClick(args);					
				}
			}

			private void HandleIEContextMenu(object sender, HtmlEventArgs args)
			{
				OnRightClick(args);
			}

			private void OnRightClick(HtmlEventArgs args)
			{
				if (RightClick != null)
				{
					RightClick(null, args);
				}
			}
		}
	}
}
