using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class DeleteObjectsByRegex : EditorWindow
{
	private static GameObject parentObject;
	private static string pattern = "";
	
	// ヒエラルキーのオブジェクト右クリックで使えるやつ
	[MenuItem ("GameObject/Delete Objects By Regex", false, 20)]
	public static void ShowWindow () {
		parentObject = Selection.activeGameObject;
		EditorWindow.GetWindow (typeof (DeleteObjectsByRegex));
	}
	
	// 出てくるウィンドウ
	private void OnGUI()
	{
		EditorGUILayout.BeginVertical(GUI.skin.box);
		EditorGUILayout.LabelField("すべての子要素を正規表現で削除します。");
		GUILayout.Space(5);
		EditorGUILayout.BeginHorizontal();
		parentObject = (GameObject)EditorGUILayout.ObjectField("Parent", parentObject, typeof(GameObject), true);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		pattern = EditorGUILayout.TextField("pattern", pattern);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();

		if (GUILayout.Button("正規表現で削除する"))
		{
			exec();
		}
	}

	// 削除処理
	private void exec()
	{
		StringBuilder  sb = new StringBuilder("Deleted Objects:\n");
		
		Regex regex = new Regex (pattern);
		
		// 対象要素を列挙する
		List<GameObject> children = new List<GameObject> ();
		GetChildren (parentObject, ref children, ref regex);
		
		// 対象要素をたどる
		foreach(GameObject child in children)
		{
			sb.AppendFormat("{0}\n", child.name);
			// Undoできるように削除する
			Undo.DestroyObjectImmediate(child);
		}

		EditorUtility.DisplayDialog("完了", sb.ToString(), "OK");

		return;
	}

	public static void GetChildren (GameObject obj, ref List<GameObject> list, ref Regex regex)
	{
		// 子要素を取得する
		Transform children = obj.GetComponentInChildren<Transform> ();
		foreach (Transform child in children) {
			// 正規表現マッチでリストに追加する
			if(regex.IsMatch(child.gameObject.name))
			{
				list.Add (child.gameObject);
			}
			GetChildren (child.gameObject, ref list, ref regex);
		}
	}
}
