namespace AK.Wwise.TreeView
{
	[System.Serializable]
	public class TreeViewItem
	{
		public enum SiblingOrder
		{
			FIRST_CHILD,
			MIDDLE_CHILD,
			LAST_CHILD
		}

		public enum TextureIcons
		{
			BLANK,
			GUIDE,
			LAST_SIBLING_COLLAPSED,
			LAST_SIBLING_EXPANDED,
			LAST_SIBLING_NO_CHILD,
			MIDDLE_SIBLING_COLLAPSED,
			MIDDLE_SIBLING_EXPANDED,
			MIDDLE_SIBLING_NO_CHILD,
			NORMAL_CHECKED,
			NORMAL_UNCHECKED
		}

		private static int s_clickCount;
		public System.EventHandler Checked = null;
		public System.EventHandler Click = null;
		public System.EventHandler CustomIconBuilder = null;
		public object DataContext;
		public System.EventHandler Dragged = null;
		public string Header = string.Empty;
		public bool IsCheckBox;
		public bool IsChecked;
		public bool IsDraggable;
		public bool IsExpanded;
		public bool IsHidden = false;
		public bool IsHover;
		public bool IsSelected;
		public System.Collections.Generic.List<TreeViewItem> Items = new System.Collections.Generic.List<TreeViewItem>();

		/// <summary>
		///     The distance to the hover item
		/// </summary>
		private float m_hoverTime;

		public TreeViewItem Parent;

		public TreeViewControl ParentControl;
		public System.EventHandler Selected = null;
		public System.EventHandler Unchecked = null;
		public System.EventHandler Unselected = null;

		public TreeViewItem(TreeViewControl parentControl, TreeViewItem parent)
		{
			ParentControl = parentControl;
			Parent = parent;
		}

		public TreeViewItem AddItem(string header)
		{
			var item = new TreeViewItem(ParentControl, this) { Header = header };
			Items.Add(item);
			return item;
		}

		public TreeViewItem AddItem(string header, object context)
		{
			var item = new TreeViewItem(ParentControl, this) { Header = header, DataContext = context };
			Items.Add(item);
			return item;
		}

		public TreeViewItem AddItem(string header, object context, bool in_isExpended)
		{
			var item = new TreeViewItem(ParentControl, this)
			{
				Header = header,
				DataContext = context,
				IsExpanded = in_isExpended
			};
			Items.Add(item);
			return item;
		}

		public TreeViewItem AddItem(string header, bool isExpanded)
		{
			var item = new TreeViewItem(ParentControl, this) { Header = header, IsExpanded = isExpanded };
			Items.Add(item);
			return item;
		}

		public TreeViewItem AddItem(string header, bool isDraggable, bool isExpanded, object context)
		{
			var item = new TreeViewItem(ParentControl, this)
			{
				Header = header,
				IsDraggable = isDraggable,
				IsExpanded = isExpanded,
				DataContext = context
			};
			Items.Add(item);
			return item;
		}

		public TreeViewItem AddItem(string header, bool isExpanded, bool isChecked)
		{
			var item = new TreeViewItem(ParentControl, this)
			{
				Header = header,
				IsExpanded = isExpanded,
				IsCheckBox = true,
				IsChecked = isChecked
			};
			Items.Add(item);
			return item;
		}

		public TreeViewItem FindItemByName(string name)
		{
			foreach (var item in Items)
			{
				if (item.Header == name)
					return item;
			}

			return null;
		}

		public bool HasChildItems()
		{
			return null != Items && Items.Count > 0;
		}

		private float CalculateHoverTime(UnityEngine.Rect rect, UnityEngine.Vector3 mousePos)
		{
			if (rect.Contains(mousePos))
				return 0f;

			var midPoint = (rect.yMin + rect.yMax) * 0.5f;
			var pointA = mousePos.y;
			return UnityEngine.Mathf.Abs(midPoint - pointA) / 50f;
		}

		private void SetIconExpansion(SiblingOrder siblingOrder, TextureIcons middle, TextureIcons last)
		{
			var result = false;
			switch (siblingOrder)
			{
				case SiblingOrder.FIRST_CHILD:
				case SiblingOrder.MIDDLE_CHILD:
					result = ParentControl.Button(middle);
					break;
				case SiblingOrder.LAST_CHILD:
					result = ParentControl.Button(last);
					break;
			}

			if (result)
				IsExpanded = !IsExpanded;
		}

		public void DisplayItem(int levels, SiblingOrder siblingOrder)
		{
			if (null == ParentControl || IsHidden)
				return;

			var clicked = false;

			using (new UnityEngine.GUILayout.HorizontalScope())
			{
				for (var index = 0; index < levels; ++index)
					ParentControl.Button(TextureIcons.GUIDE);

				if (!HasChildItems())
					SetIconExpansion(siblingOrder, TextureIcons.MIDDLE_SIBLING_NO_CHILD, TextureIcons.LAST_SIBLING_NO_CHILD);
				else if (IsExpanded)
					SetIconExpansion(siblingOrder, TextureIcons.MIDDLE_SIBLING_EXPANDED, TextureIcons.LAST_SIBLING_EXPANDED);
				else
					SetIconExpansion(siblingOrder, TextureIcons.MIDDLE_SIBLING_COLLAPSED, TextureIcons.LAST_SIBLING_COLLAPSED);

				// display the text for the tree view
				if (!string.IsNullOrEmpty(Header))
				{
					var isSelected = ParentControl.SelectedItem == this && !ParentControl.m_forceDefaultSkin;
					if (isSelected)
						UnityEngine.GUI.skin = ParentControl.m_skinSelected;

					if (IsCheckBox)
					{
						if (IsChecked && ParentControl.Button(TextureIcons.NORMAL_CHECKED))
						{
							IsChecked = false;
							if (ParentControl.SelectedItem != this)
							{
								ParentControl.SelectedItem = this;
								IsSelected = true;
								if (null != Selected)
									Selected.Invoke(this, new SelectedEventArgs());
							}

							if (null != Unchecked)
								Unchecked.Invoke(this, new UncheckedEventArgs());
						}
						else if (!IsChecked && ParentControl.Button(TextureIcons.NORMAL_UNCHECKED))
						{
							IsChecked = true;
							if (ParentControl.SelectedItem != this)
							{
								ParentControl.SelectedItem = this;
								IsSelected = true;
								if (null != Selected)
									Selected.Invoke(this, new SelectedEventArgs());
							}

							if (null != Checked)
								Checked.Invoke(this, new CheckedEventArgs());
						}

						ParentControl.Button(TextureIcons.BLANK);
					}

					// Add a custom icon, if any
					if (null != CustomIconBuilder)
					{
						CustomIconBuilder.Invoke(this, new CustomIconEventArgs());
						ParentControl.Button(TextureIcons.BLANK);
					}

					if (UnityEngine.Event.current.isMouse)
						s_clickCount = UnityEngine.Event.current.clickCount;

					if (ParentControl.IsHoverEnabled)
					{
						var oldSkin = UnityEngine.GUI.skin;
						UnityEngine.GUI.skin = isSelected ? ParentControl.m_skinSelected :
							IsHover ? ParentControl.m_skinHover : ParentControl.m_skinUnselected;

						if (ParentControl.IsHoverAnimationEnabled)
							UnityEngine.GUI.skin.button.fontSize = (int) UnityEngine.Mathf.Lerp(20f, 12f, m_hoverTime);

						UnityEngine.GUI.SetNextControlName("toggleButton"); //workaround to dirty GUI
						if (UnityEngine.GUILayout.Button(Header))
						{
							UnityEngine.GUI.FocusControl("toggleButton"); //workaround to dirty GUI
							if (ParentControl.SelectedItem != this)
							{
								ParentControl.SelectedItem = this;
								IsSelected = true;
								if (null != Selected)
									Selected.Invoke(this, new SelectedEventArgs());
							}

							if (null != Click && (uint) s_clickCount <= 2)
								clicked = true;
						}

						UnityEngine.GUI.skin = oldSkin;
					}
					else
					{
						UnityEngine.GUI.SetNextControlName("toggleButton"); //workaround to dirty GUI
						if (UnityEngine.GUILayout.RepeatButton(Header))
						{
							UnityEngine.GUI.FocusControl("toggleButton"); //workaround to dirty GUI
							if (ParentControl.SelectedItem != this)
							{
								ParentControl.SelectedItem = this;
								IsSelected = true;
								if (null != Selected)
									Selected.Invoke(this, new SelectedEventArgs());
							}

							if (null != Click && (uint) s_clickCount <= 2)
								clicked = true;
						}
					}

					if (isSelected && !ParentControl.m_forceDefaultSkin)
						UnityEngine.GUI.skin = ParentControl.m_skinUnselected;
				}
			}

			if (ParentControl.IsHoverEnabled)
			{
				if (null != UnityEngine.Event.current && UnityEngine.Event.current.type == UnityEngine.EventType.Repaint)
				{
					var mousePos = UnityEngine.Event.current.mousePosition;
					if (ParentControl.HasFocus(mousePos))
					{
						var lastRect = UnityEngine.GUILayoutUtility.GetLastRect();
						IsHover = lastRect.Contains(mousePos);
						if (IsHover)
							ParentControl.HoverItem = this;

						if (ParentControl.IsHoverEnabled && ParentControl.IsHoverAnimationEnabled)
							m_hoverTime = CalculateHoverTime(lastRect, UnityEngine.Event.current.mousePosition);
					}
				}
			}

			if (HasChildItems() && IsExpanded)
			{
				for (var index = 0; index < Items.Count; ++index)
				{
					var child = Items[index];
					child.Parent = this;
					child.DisplayItem(levels + 1,
						index + 1 == Items.Count ? SiblingOrder.LAST_CHILD :
						index == 0 ? SiblingOrder.FIRST_CHILD : SiblingOrder.MIDDLE_CHILD);
				}
			}

			if (clicked)
				Click.Invoke(this, new ClickEventArgs((uint) s_clickCount));

			if (IsSelected && ParentControl.SelectedItem != this && null != Unselected)
				Unselected.Invoke(this, new UnselectedEventArgs());

			IsSelected = ParentControl.SelectedItem == this;

			if (IsDraggable)
				HandleGUIEvents();
		}

		private void HandleGUIEvents()
		{
			// Handle events
			var evt = UnityEngine.Event.current;
			var currentEventType = evt.type;

			if (currentEventType == UnityEngine.EventType.MouseDrag)
			{
				if (null != Dragged)
				{
					try
					{
						UnityEditor.DragAndDrop.PrepareStartDrag();
						Dragged.Invoke(ParentControl.SelectedItem, new DragEventArgs());
						evt.Use();
					}
					catch (System.Exception e)
					{
						UnityEngine.Debug.Log(e);
					}
				}
			}
		}

		public class ClickEventArgs : System.EventArgs
		{
			public uint m_clickCount;

			public ClickEventArgs(uint in_clickCount)
			{
				m_clickCount = in_clickCount;
			}
		}

		public class CheckedEventArgs : System.EventArgs
		{
		}

		public class UncheckedEventArgs : System.EventArgs
		{
		}

		public class SelectedEventArgs : System.EventArgs
		{
		}

		public class UnselectedEventArgs : System.EventArgs
		{
		}

		public class DragEventArgs : System.EventArgs
		{
		}

		public class CustomIconEventArgs : System.EventArgs
		{
		}
	}
}