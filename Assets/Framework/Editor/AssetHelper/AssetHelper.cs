using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/* Author : Altaf
 * Date : May 20, 2013
 * Purpose : Context menu to copy, cut & paste items
*/

namespace XcelerateGames.Editor
{
    public class AssetHelper : EditorWindow
    {
        private static List<string> _AssetPaths = new List<string>();
        private static List<string> _Clipboard = new List<string>();
        private Vector2 mScroll = Vector2.zero;

        private static bool mIsCopy = true;

        [MenuItem("Assets/Copy", false, 80)]
        private static void CopyAsset()
        {
            foreach (Object obj in Selection.objects)
                _AssetPaths.Add(AssetDatabase.GetAssetPath(obj));
            mIsCopy = true;
            Debug.Log("Copied " + _AssetPaths.Count + " asset(s).");
        }

        [MenuItem("Assets/Copy", true)]
        private static bool CopyAssetValidate()
        {
            return (Selection.objects != null && Selection.objects.Length > 0);
        }

        [MenuItem("Assets/Cut", false, 80)]
        private static void MoveAsset()
        {
            foreach (Object obj in Selection.objects)
                _AssetPaths.Add(AssetDatabase.GetAssetPath(obj));
            mIsCopy = false;
            Debug.Log("Copied " + _AssetPaths.Count + " asset(s).");
        }

        [MenuItem("Assets/Cut", true)]
        private static bool MoveAssetValidate()
        {
            return (Selection.objects != null && Selection.objects.Length > 0);
        }

        [MenuItem("Assets/Paste", false, 80)]
        private static void PasteAsset()
        {
            string dstPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            string fileExt = System.IO.Path.GetExtension(dstPath);
            if (!string.IsNullOrEmpty(fileExt))
                dstPath = System.IO.Path.GetDirectoryName(dstPath);
            DoPasteAsset(dstPath);
        }

        private static void DoPasteAsset(string dstPath)
        {
            Debug.Log("Copying asset(s) to " + dstPath);
            foreach (string assetPath in _AssetPaths)
            {
                string fileName = System.IO.Path.GetFileName(assetPath);
                string msg = "";
                if (mIsCopy)
                {
                    bool copied = AssetDatabase.CopyAsset(assetPath, dstPath + "/" + fileName);
                    if (!copied)
                        msg = "Failed to copy";
                }
                else
                    msg = AssetDatabase.MoveAsset(assetPath, dstPath + "/" + fileName);
                if (string.IsNullOrEmpty(msg))
                    Debug.Log("Pasted asset at path : " + (dstPath + "/" + fileName));
                else
                    Debug.LogError("Failed to paste asset : " + msg);
            }

            _AssetPaths.Clear();
        }

        [MenuItem("Assets/Paste", true)]
        private static bool PasteAssetValidate()
        {
            //Have we copied anything?
            if (_AssetPaths.Count == 0)
                return false;
            //Try to paste no where?
            if (Selection.activeObject == null)
                return false;
            //Trying to paste on same asset again?
            //		if(AssetDatabase.GetAssetPath(Selection.activeObject) == _AssetPaths)
            //			return false;

            return true;
        }

        [MenuItem("Assets/Clipboard/Paste", false, 81)]
        private static void DoSetAssetBundleName()
        {
            GetWindow<AssetHelper>();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Pick Location");
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            mScroll = GUILayout.BeginScrollView(mScroll);
            foreach (string dstPath in _Clipboard)
            {
                if (GUILayout.Button(dstPath))
                    DoPasteAsset(dstPath);
            }
            GUILayout.EndScrollView();
        }

        [MenuItem("Assets/Clipboard/Add", false, 81)]
        private static void ClipboardAdd()
        {
            foreach (Object obj in Selection.objects)
            {
                string dstPath = AssetDatabase.GetAssetPath(obj);
                string fileExt = System.IO.Path.GetExtension(dstPath);
                if (!string.IsNullOrEmpty(fileExt))
                    dstPath = System.IO.Path.GetDirectoryName(dstPath);
                if (!_Clipboard.Contains(dstPath))
                    _Clipboard.Add(dstPath);
            }
        }

        [MenuItem("Assets/Clipboard/Add", true)]
        private static bool ClipboardAddValidate()
        {
            return (Selection.objects != null && Selection.objects.Length > 0);
        }

        [MenuItem("Assets/Clipboard/Remove", false, 81)]
        private static void ClipboardRemove()
        {
            foreach (Object obj in Selection.objects)
            {
                string dstPath = AssetDatabase.GetAssetPath(obj);
                string fileExt = System.IO.Path.GetExtension(dstPath);
                if (!string.IsNullOrEmpty(fileExt))
                    dstPath = System.IO.Path.GetDirectoryName(dstPath);
                _Clipboard.Remove(dstPath);
            }
        }

        [MenuItem("Assets/Clipboard/Remove", true)]
        private static bool ClipboardRemoveValidate()
        {
            return (Selection.objects != null && Selection.objects.Length > 0);
        }

        [MenuItem("Assets/Clipboard/Clear", false, 81)]
        private static void ClipboardClear()
        {
            _Clipboard.Clear();
        }
    }
}