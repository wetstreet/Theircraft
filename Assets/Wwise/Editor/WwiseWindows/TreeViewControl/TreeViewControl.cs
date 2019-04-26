namespace AK.Wwise.TreeView
{
	public class TreeViewControl
	{
		public enum DisplayTypes
		{
			NONE, //used by the inspector
			USE_SCROLL_VIEW, //used by panels
			USE_SCROLL_AREA //used by gameview, sceneview
		}

		public int Height = 400;

		/// <summary>
		///     The selected item
		/// </summary>
		public TreeViewItem HoverItem = null;

		public bool IsExpanded = false;
		public bool IsHoverAnimationEnabled = false;
		public bool IsHoverEnabled = false;

		/// <summary>
		///     Force to use the button text
		/// </summary>
		public bool m_forceButtonText = false;

		/// <summary>
		///     Use the default skin
		/// </summary>
		public bool m_forceDefaultSkin = false;

		/// <summary>
		///     The root item
		/// </summary>
		public TreeViewItem m_roomItem;

		/// <summary>
		///     Handle the unity scrolling vector
		/// </summary>
		protected UnityEngine.Vector2 m_scrollView = UnityEngine.Vector2.zero;


		/// <summary>
		///     Skin used by the tree view
		/// </summary>
		public UnityEngine.GUISkin m_skinHover;

		public UnityEngine.GUISkin m_skinSelected;
		public UnityEngine.GUISkin m_skinUnselected;

		/// <summary>
		///     Texture skin references
		/// </summary>
		public UnityEngine.Texture2D m_textureBlank;

		public UnityEngine.Texture2D m_textureGuide;
		public UnityEngine.Texture2D m_textureLastSiblingCollapsed;
		public UnityEngine.Texture2D m_textureLastSiblingExpanded;
		public UnityEngine.Texture2D m_textureLastSiblingNoChild;
		public UnityEngine.Texture2D m_textureMiddleSiblingCollapsed;
		public UnityEngine.Texture2D m_textureMiddleSiblingExpanded;
		public UnityEngine.Texture2D m_textureMiddleSiblingNoChild;
		public UnityEngine.Texture2D m_textureNormalChecked;
		public UnityEngine.Texture2D m_textureNormalUnchecked;
		public UnityEngine.Texture2D m_textureSelectedBackground;
		public TreeViewItem SelectedItem;
		public int Width = 400;
		public int X = 0;
		public int Y = 0;

		public TreeViewItem RootItem
		{
			get
			{
				if (null == m_roomItem)
					m_roomItem = new TreeViewItem(this, null) { Header = "Root item" };

				return m_roomItem;
			}
		}

		/// <summary>
		///     Accesses the root item header
		/// </summary>
		public string Header
		{
			get { return RootItem.Header; }
			set { RootItem.Header = value; }
		}

		/// <summary>
		///     Accesses the root data context
		/// </summary>
		public object DataContext
		{
			get { return RootItem.DataContext; }
			set { RootItem.DataContext = value; }
		}

		/// <summary>
		///     Accessor to the root items
		/// </summary>
		public System.Collections.Generic.List<TreeViewItem> Items
		{
			get { return RootItem.Items; }
			set { RootItem.Items = value; }
		}

		private void Start()
		{
			SelectedItem = null;
		}

		/// <summary>
		///     Show the button texture
		/// </summary>
		/// <param name="texture">
		///     A <see cref="UnityEngine.Texture2D" />
		/// </param>
		/// <returns>
		///     A <see cref="System.Boolean" />
		/// </returns>
		protected bool ShowButtonTexture(UnityEngine.Texture2D texture)
		{
			return UnityEngine.GUILayout.Button(texture, UnityEngine.GUILayout.MaxWidth(texture.width),
				UnityEngine.GUILayout.MaxHeight(texture.height));
		}

		/// <summary>
		///     Find the button texture/text by enum
		/// </summary>
		/// <param name="item"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <returns></returns>
		public bool Button(TreeViewItem.TextureIcons item)
		{
			switch (item)
			{
				case TreeViewItem.TextureIcons.BLANK:
					if (null == m_textureGuide || m_forceButtonText)
						UnityEngine.GUILayout.Label("", UnityEngine.GUILayout.MaxWidth(4));
					else
					{
						UnityEngine.GUILayout.Label(m_textureBlank, UnityEngine.GUILayout.MaxWidth(4),
							UnityEngine.GUILayout.MaxHeight(16));
					}

					return false;

				case TreeViewItem.TextureIcons.GUIDE:
					if (null == m_textureGuide || m_forceButtonText)
						UnityEngine.GUILayout.Label("|", UnityEngine.GUILayout.MaxWidth(16));
					else
					{
						UnityEngine.GUILayout.Label(m_textureGuide, UnityEngine.GUILayout.MaxWidth(16),
							UnityEngine.GUILayout.MaxHeight(16));
					}

					return false;

				case TreeViewItem.TextureIcons.LAST_SIBLING_COLLAPSED:
					if (null == m_textureLastSiblingCollapsed || m_forceButtonText)
						return UnityEngine.GUILayout.Button("<", UnityEngine.GUILayout.MaxWidth(16));
					else
						return ShowButtonTexture(m_textureLastSiblingCollapsed);

				case TreeViewItem.TextureIcons.LAST_SIBLING_EXPANDED:
					if (null == m_textureLastSiblingExpanded || m_forceButtonText)
						return UnityEngine.GUILayout.Button(">", UnityEngine.GUILayout.MaxWidth(16));
					else
						return ShowButtonTexture(m_textureLastSiblingExpanded);

				case TreeViewItem.TextureIcons.LAST_SIBLING_NO_CHILD:
					if (null == m_textureLastSiblingNoChild || m_forceButtonText)
						return UnityEngine.GUILayout.Button("-", UnityEngine.GUILayout.MaxWidth(16));
					else
						return UnityEngine.GUILayout.Button(m_textureLastSiblingNoChild, UnityEngine.GUILayout.MaxWidth(16));

				case TreeViewItem.TextureIcons.MIDDLE_SIBLING_COLLAPSED:
					if (null == m_textureMiddleSiblingCollapsed || m_forceButtonText)
						return UnityEngine.GUILayout.Button("<", UnityEngine.GUILayout.MaxWidth(16));
					else
						return ShowButtonTexture(m_textureMiddleSiblingCollapsed);

				case TreeViewItem.TextureIcons.MIDDLE_SIBLING_EXPANDED:
					if (null == m_textureMiddleSiblingExpanded || m_forceButtonText)
						return UnityEngine.GUILayout.Button(">", UnityEngine.GUILayout.MaxWidth(16));
					else
						return UnityEngine.GUILayout.Button(m_textureMiddleSiblingExpanded, UnityEngine.GUILayout.MaxWidth(16));

				case TreeViewItem.TextureIcons.MIDDLE_SIBLING_NO_CHILD:
					if (null == m_textureMiddleSiblingNoChild || m_forceButtonText)
						return UnityEngine.GUILayout.Button("-", UnityEngine.GUILayout.MaxWidth(16));
					else
						return ShowButtonTexture(m_textureMiddleSiblingNoChild);

				case TreeViewItem.TextureIcons.NORMAL_CHECKED:
					if (null == m_textureNormalChecked || m_forceButtonText)
						return UnityEngine.GUILayout.Button("x", UnityEngine.GUILayout.MaxWidth(16));
					else
						return UnityEngine.GUILayout.Button(m_textureNormalChecked, UnityEngine.GUILayout.MaxWidth(16));

				case TreeViewItem.TextureIcons.NORMAL_UNCHECKED:
					if (null == m_textureNormalUnchecked || m_forceButtonText)
						return UnityEngine.GUILayout.Button("o", UnityEngine.GUILayout.MaxWidth(16));
					else
						return ShowButtonTexture(m_textureNormalUnchecked);

				default:
					return false;
			}
		}

		/// <summary>
		///     Called from OnGUI or EditorWindow.OnGUI
		/// </summary>
		public virtual void DisplayTreeView(DisplayTypes displayType)
		{
			using (new UnityEngine.GUILayout.HorizontalScope("box"))
			{
				AssignDefaults();
				if (!m_forceDefaultSkin)
					ApplySkinKeepingScrollbars();

				switch (displayType)
				{
					case DisplayTypes.USE_SCROLL_VIEW:
						using (var scope = new UnityEngine.GUILayout.ScrollViewScope(m_scrollView)
						) //, GUILayout.MaxWidth(Width), GUILayout.MaxHeight(Height));
						{
							m_scrollView = scope.scrollPosition;
							RootItem.DisplayItem(0, TreeViewItem.SiblingOrder.FIRST_CHILD);
						}

						break;
					//case TreeViewControl.DisplayTypes.USE_SCROLL_AREA:
					//	using (var area = new GUILayout.AreaScope(new Rect(X, Y, Width, Height)))
					//	using (var scope = new GUILayout.ScrollViewScope(m_scrollView))//, GUILayout.MaxWidth(Width), GUILayout.MaxHeight(Height));
					//	{
					//		m_scrollView = scope.scrollPosition;
					//		RootItem.DisplayItem(0, TreeViewItem.SiblingOrder.FIRST_CHILD);
					//	}
					//	break;
					default:
						RootItem.DisplayItem(0, TreeViewItem.SiblingOrder.FIRST_CHILD);
						break;
				}

				UnityEngine.GUI.skin = null;
			}
		}

		private void ApplySkinKeepingScrollbars()
		{
			var hScroll = UnityEngine.GUI.skin.horizontalScrollbar;
			var hScrollDButton = UnityEngine.GUI.skin.horizontalScrollbarLeftButton;
			var hScrollUButton = UnityEngine.GUI.skin.horizontalScrollbarRightButton;
			var hScrollThumb = UnityEngine.GUI.skin.horizontalScrollbarThumb;
			var vScroll = UnityEngine.GUI.skin.verticalScrollbar;
			var vScrollDButton = UnityEngine.GUI.skin.verticalScrollbarDownButton;
			var vScrollUButton = UnityEngine.GUI.skin.verticalScrollbarUpButton;
			var vScrollThumb = UnityEngine.GUI.skin.verticalScrollbarThumb;

			UnityEngine.GUI.skin = m_skinUnselected;

			UnityEngine.GUI.skin.horizontalScrollbar = hScroll;
			UnityEngine.GUI.skin.horizontalScrollbarLeftButton = hScrollDButton;
			UnityEngine.GUI.skin.horizontalScrollbarRightButton = hScrollUButton;
			UnityEngine.GUI.skin.horizontalScrollbarThumb = hScrollThumb;
			UnityEngine.GUI.skin.verticalScrollbar = vScroll;
			UnityEngine.GUI.skin.verticalScrollbarDownButton = vScrollDButton;
			UnityEngine.GUI.skin.verticalScrollbarUpButton = vScrollUButton;
			UnityEngine.GUI.skin.verticalScrollbarThumb = vScrollThumb;
		}

		public bool HasFocus(UnityEngine.Vector2 mousePos)
		{
			var rect = new UnityEngine.Rect(m_scrollView.x, m_scrollView.y, 600, 900); // Width, Height);
			return rect.Contains(mousePos);
		}

		public void ApplySkin()
		{
			// create new skin instance
			var skinHover = UnityEngine.Object.Instantiate(m_skinHover);
			var skinSelected = UnityEngine.Object.Instantiate(m_skinSelected);
			var skinUnselected = UnityEngine.Object.Instantiate(m_skinUnselected);

			// name the skins
			skinHover.name = "Hover";
			skinSelected.name = "Selected";
			skinUnselected.name = "Unselected";

			m_skinHover = skinHover;
			m_skinSelected = skinSelected;
			m_skinUnselected = skinUnselected;
		}

		public virtual void AssignDefaults()
		{
			// create new skin instance
			var skinHover = UnityEngine.ScriptableObject.CreateInstance<UnityEngine.GUISkin>();
			var skinSelected = UnityEngine.ScriptableObject.CreateInstance<UnityEngine.GUISkin>();
			var skinUnselected = UnityEngine.ScriptableObject.CreateInstance<UnityEngine.GUISkin>();
			skinHover.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
			skinSelected.hideFlags = UnityEngine.HideFlags.HideAndDontSave;
			skinUnselected.hideFlags = UnityEngine.HideFlags.HideAndDontSave;

			// name the skins
			skinHover.name = "Hover";
			skinSelected.name = "Selected";
			skinUnselected.name = "Unselected";

			var tempWwisePath = "Assets/Wwise/Editor/WwiseWindows/TreeViewControl/";

			m_textureBlank = GetTexture(tempWwisePath + "blank.png");
			m_textureGuide = GetTexture(tempWwisePath + "guide.png");
			m_textureLastSiblingCollapsed = GetTexture(tempWwisePath + "last_sibling_collapsed.png");
			m_textureLastSiblingExpanded = GetTexture(tempWwisePath + "last_sibling_expanded.png");
			m_textureLastSiblingNoChild = GetTexture(tempWwisePath + "last_sibling_nochild.png");
			m_textureMiddleSiblingCollapsed = GetTexture(tempWwisePath + "middle_sibling_collapsed.png");
			m_textureMiddleSiblingExpanded = GetTexture(tempWwisePath + "middle_sibling_expanded.png");
			m_textureMiddleSiblingNoChild = GetTexture(tempWwisePath + "middle_sibling_nochild.png");
			m_textureNormalChecked = GetTexture(tempWwisePath + "normal_checked.png");
			m_textureNormalUnchecked = GetTexture(tempWwisePath + "normal_unchecked.png");
			m_textureSelectedBackground = GetTexture(tempWwisePath + "selected_background_color.png");

			m_skinHover = skinHover;
			m_skinSelected = skinSelected;
			m_skinUnselected = skinUnselected;

			SetBackground(m_skinHover.button, null);
			SetBackground(m_skinHover.toggle, null);
			SetButtonFontSize(m_skinHover.button);
			SetButtonFontSize(m_skinHover.toggle);
			RemoveMargins(m_skinHover.button);
			RemoveMargins(m_skinHover.toggle);
			SetTextColor(m_skinHover.button, UnityEngine.Color.yellow);
			SetTextColor(m_skinHover.toggle, UnityEngine.Color.yellow);

			SetBackground(m_skinSelected.button, m_textureSelectedBackground);
			SetBackground(m_skinSelected.toggle, m_textureSelectedBackground);
			SetButtonFontSize(m_skinSelected.button);
			SetButtonFontSize(m_skinSelected.toggle);
			RemoveMargins(m_skinSelected.button);
			RemoveMargins(m_skinSelected.toggle);
			SetTextColor(m_skinSelected.button, UnityEngine.Color.yellow);
			SetTextColor(m_skinSelected.toggle, UnityEngine.Color.yellow);

			SetBackground(m_skinUnselected.button, null);
			SetBackground(m_skinUnselected.toggle, null);
			SetButtonFontSize(m_skinUnselected.button);
			SetButtonFontSize(m_skinUnselected.toggle);
			RemoveMargins(m_skinUnselected.button);
			RemoveMargins(m_skinUnselected.toggle);

			if (UnityEngine.Application.HasProLicense())
			{
				SetTextColor(m_skinUnselected.button, UnityEngine.Color.white);
				SetTextColor(m_skinUnselected.toggle, UnityEngine.Color.white);
			}
			else
			{
				SetTextColor(m_skinUnselected.button, UnityEngine.Color.black);
				SetTextColor(m_skinUnselected.toggle, UnityEngine.Color.black);
			}
		}

		private void SetBackground(UnityEngine.GUIStyle style, UnityEngine.Texture2D texture)
		{
			style.active.background = texture;
			style.focused.background = texture;
			style.hover.background = texture;
			style.normal.background = texture;
			style.onActive.background = texture;
			style.onFocused.background = texture;
			style.onHover.background = texture;
			style.onNormal.background = texture;
		}

		private void SetTextColor(UnityEngine.GUIStyle style, UnityEngine.Color color)
		{
			style.active.textColor = color;
			style.focused.textColor = color;
			style.hover.textColor = color;
			style.normal.textColor = color;
			style.onActive.textColor = color;
			style.onFocused.textColor = color;
			style.onHover.textColor = color;
			style.onNormal.textColor = color;
		}

		private void RemoveMargins(UnityEngine.GUIStyle style)
		{
			style.margin.bottom = 0;
			style.margin.left = 0;
			style.margin.right = 0;
			style.margin.top = 0;
		}

		private void SetButtonFontSize(UnityEngine.GUIStyle style)
		{
			style.fontSize = 12;
		}

		protected UnityEngine.Texture2D GetTexture(string texturePath)
		{
			try
			{
#if UNITY_EDITOR_MAC
				var importer = UnityEditor.AssetImporter.GetAtPath(texturePath) as UnityEditor.TextureImporter;
				importer.textureType = UnityEditor.TextureImporterType.Cursor;
				UnityEditor.AssetDatabase.WriteImportSettingsIfDirty(texturePath);
#endif

				return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Texture2D>(texturePath);
			}
			catch (System.Exception ex)
			{
				UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to find local texture: {0}", ex));
				return null;
			}
		}
	}
}