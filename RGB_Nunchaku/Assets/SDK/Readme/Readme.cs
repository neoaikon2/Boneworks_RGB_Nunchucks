﻿using System;
using UnityEngine;

public class Readme : ScriptableObject {
	public Texture2D icon;
	public string title;
	public Section[] sections;
	public bool loadedLayout;
	
	[Serializable]
	public class Section {
		public string largeHeading;
		public string heading;
		[Multiline] public string text;
		public string linkText;
		public string url;
	}
}
