using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[InitializeOnLoad]
public class SublimeUnity : MonoBehaviour 
{
	const string version = "0.2";
	static string sublimePath = "";
	static string defaultSublimeProjectPath = "Assets";

	static SublimeUnity()
	{
		if( !Directory.Exists( GetPersistentPath() + "SublimeUnity/"))
		{
			Directory.CreateDirectory( GetPersistentPath() + "SublimeUnity/");

			CheckSublimeUnityTxt();
		}
		else
		{
			if( !CheckSublimeUnityTxt())
			{
				string[] s = File.ReadAllLines( GetSublimeUnityPath());
				sublimePath = s[1];
			}
		}
	}

	[MenuItem( "Sublime Unity/Open Sublime Project %&o", false, 1)]
	static void OpenSublime()
	{
		if( File.Exists( GetSublimeProjectName()))
		{
				System.Diagnostics.Process.Start( sublimePath, "--project " + GetSublimeProjectName());
		}
		else
		{
			GenerateFile();
			OpenSublime();
		}
	}

	[MenuItem( "Sublime Unity/Generate .sublime-project %&p", false, 1)]
	static void GenerateFile() 
	{
		File.WriteAllText( GetSublimeProjectName(), GetSublimeProjectFile());
		Debug.Log("<b>Created file:</b> <color=#008000ff>"+GetUnityProjectName()+"</color>");
	}

	[MenuItem( "Sublime Unity/Settings/Change Sublime Path", false, 51)]
	static void ChangeSublimeTextPath()
	{
		if( Application.platform == RuntimePlatform.OSXEditor)
		{
			if( EditorUtility.DisplayDialog("OS X Compatibility Error", 
											"On OSX we need a file inside the Sublime Text.app\n"+ 
											"You can edit the SublimeUnity.txt with your custom path,\n"+
											"However installing Sublime to its default path is recommended", 
											"Ok, let me fiddle",
											"I'll install Sublime to the default path"))
			{
				OpenInFileBrowser.Open( GetSublimeUnityPath());
			}
		}
		else
		{
			string s = EditorUtility.OpenFilePanel("Select Sublime Executable", "", "");
			if( s != "")
			{
				sublimePath = s;
			}
		}
	}

	[MenuItem( "Sublime Unity/Settings/Change .sublime-project top folder", false, 52)]
	static void ChangeSublimeProjectTopFolder()
	{
		string s = EditorUtility.OpenFolderPanel("New .sublime-project Top Folder", "", "");
		if( s != "")
		{
			string projectPath = GetProjectPath();
			if( projectPath[projectPath.Length-1] == '/')
			{
				projectPath = projectPath.Remove(projectPath.Length -1);
			}
			DirectoryInfo unityDir = new DirectoryInfo( projectPath);
			DirectoryInfo newDir   = new DirectoryInfo( s);
			bool containsDir = false;
			List<string> relPath = new List<string>();
			relPath.Add( newDir.Name);

			while( newDir.Parent != null)
			{
				relPath.Add( newDir.Name);
				if( newDir.Parent.FullName == unityDir.FullName)
				{
					containsDir = true;
					break;
				}
				else
				{
					newDir = newDir.Parent;
				}
			}

			if( containsDir)
			{
				string relativePath = "";
				for( int i = relPath.Count-1; i > 0; --i)
				{
					relativePath += relPath[i] + '/';
				}

				defaultSublimeProjectPath = relativePath;
				GenerateFile();

				EditorUtility.DisplayDialog("Success", 
											"Restart Sublime for new settings to take affect.", 
											"OK");
			}
			else
			{
				if( EditorUtility.DisplayDialog("Folder Selection Error",
												"The folder you have selected\n"+
												"is not a sub-folder of your Unity Project",
												"Try Again"))
				{
					ChangeSublimeProjectTopFolder();
				}
			}
		}
	}

	static string GetPersistentPath()
	{
		string[] s = Application.persistentDataPath.Split('/');
		string path = "";
		for( int i = 0; i < s.Length - 2; ++i)
		{
			path += s[i] + "/";
		}

		return path;
	}

	static string GetSublimeUnityPath()
	{
		return GetPersistentPath() + "SublimeUnity/SublimeUnity.txt";
	}

	static string GetSublimeProjectName()
	{
		return (GetProjectPath() + GetUnityProjectName() + ".sublime-project");
	}

	static string GetUnityProjectName()
	{
		string[] s = Application.dataPath.Split('/');
		string projectName = s[s.Length - 2];
		return projectName;
	}

	static string GetProjectPath()
	{
		string[] s = Application.dataPath.Split('/');
		string projectPath = "";
		for( int i = 0; i < s.Length - 1; ++i)
		{
			projectPath += s[i] + "/";
		}

		return projectPath;
	}

	static string GetDefaultSublimeTextPath( string currentPath = "")
	{
		if( currentPath == "")
		{
			if( Application.platform == RuntimePlatform.OSXEditor)
			{
				return "/Applications/Sublime Text.app/Contents/SharedSupport/bin/subl";
			}
			else
			if( Application.platform == RuntimePlatform.WindowsEditor)
			{
				return "C:/Program Files/Sublime Text 3/sublime_text.exe";
			}
			else
			{
				return "Sorry *nix person";
			}
		}
		else
		{
			return currentPath;
		}
	}

	static void CreateSublimeUnityTxt()
	{
		string fileInput = version + "\n" + GetDefaultSublimeTextPath( sublimePath);
		File.WriteAllText( GetSublimeUnityPath(), fileInput);
	}

	static bool CheckSublimeUnityTxt()
	{
		if( !File.Exists( GetSublimeUnityPath()))
		{
			CreateSublimeUnityTxt();
			sublimePath = GetDefaultSublimeTextPath();
			return true;
		}
		return false;
	}

	static string GetSublimeProjectFile()
	{
		List<string> defaultFile = new List<string>();

		defaultFile.Add("{");
		defaultFile.Add("    \"folders\":");
		defaultFile.Add("    [");
		defaultFile.Add("        {");
		defaultFile.Add("            \"follow_symlinks\": true,");
		defaultFile.Add("            \"path\": \""+defaultSublimeProjectPath+"\",");
		defaultFile.Add("            \"file_exclude_patterns\":");
		defaultFile.Add("            [");
		defaultFile.Add("                \"*.dll\",");
		defaultFile.Add("                \"*.meta\"");
		defaultFile.Add("            ]");
		defaultFile.Add("        }");
		defaultFile.Add("    ],");
		defaultFile.Add("    \"solution_file\": \"./"+GetUnityProjectName()+".sln\"");
		defaultFile.Add("}");

		string result = "";

		foreach( string line in defaultFile)
		{
			result += line + "\n";
		}

		return result;
	}
}
