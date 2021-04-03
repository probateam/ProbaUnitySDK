using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Proba.Scripts.SharedClasses;
using UnityEditor;
using UnityEngine;

namespace Proba.Scripts.Editor
{
    internal class EventRecorder : EditorWindow
    {
        #region variables

        private Vector2 scrollPos;
        private DataTable _data;

        #endregion

        /// <summary>Opens the events window.</summary>
        [MenuItem("Proba/Event Viewer")]
        private static void OpenEventsWindow()
        {
            GetWindow<EventRecorder>("PROBA Event Viewer");
        }

        /// <summary>Opens the git page.</summary>
        [MenuItem("Proba/Open Git")]
        private static void OpenGitPage()
        {
            Application.OpenURL("https://github.com/mghasemi23/Proba");
        }


        /// <summary>Adds Menu to the buttons to add Listener</summary>
        [MenuItem("CONTEXT/Button/Add Proba Listener")]
        private static void AddButtonListener()
        {
            var button = Selection.activeGameObject;
            button.AddComponent<ProbaButtonListener>();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Unsent Event List"))
            {
                DatabaseConnection.InitialConnectionString();
                _data = DatabaseConnection.GetAllEvents();
            }
            if (GUILayout.Button("Flush Database"))
            {
                DatabaseConnection.InitialConnectionString();
                DatabaseConnection.FlushDatabase();
                _data = DatabaseConnection.GetAllEvents();
            }
            EditorGUILayout.EndHorizontal();


            if (_data == null)
            {
                EditorGUILayout.LabelField("Press 'Get Event List'");
                return;
            }
            var i = 0;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (DataRow row in _data.Rows)
            {
                var id = row["ID"];
                var body = GetEventBody(row["Body"].ToString(), out var style);
                var sent = row["Sent"];
                var date = row["Date"];
                var texture = MakeTex(200, 200, GetColor(i++));

                EditorGUILayout.BeginVertical(style);
                GUILayout.Label(id + " " + body + " " + sent);
                EditorGUILayout.EndVertical();
            }

            if (i == 0)
            {
                EditorGUILayout.LabelField("Nothing To Show");
            }

            EditorGUILayout.EndScrollView();

        }

        //TODO Update UI
        /// <summary>Customize Events Bodies</summary>
        /// <param name="body">The body.</param>
        /// <param name="style">The style.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        private string GetEventBody(string body, out GUIStyle style)
        {
            var guiStyle = new GUIStyle(GUI.skin.box) { normal = { background = MakeTex(200, 200, Color.white) } };
            var eventBody = "EndSession";
            var data = JsonConvert.DeserializeObject<BaseEventDataViewModel>(body,
                new JsonSerializerSettings
                {
                    CheckAdditionalContent = true
                });
            //switch (data.Class)
            //{
            //    case "StartSessionViewModel":
            //        eventBody = "StartSessionViewModel";
            //        break;
            //    default:
            //        eventBody = "EndSession";
            //        break;
            //}
            if (!String.IsNullOrEmpty(data.Class))
            {
                eventBody = data.Class;
            }

            style = guiStyle;
            return body;
            //return eventBody;
        }

        public static Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for (var i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        public static Color GetColor(int index)
        {
            switch (index % 2)
            {
                case 0:
                    return new Color(0.83f, .83f, 0.83F);
                case 1:
                    return new Color(0.93f, .93f, 0.93F);
                default:
                    return new Color(0.83f, .83f, 0.83F);
            }
        }
    }
}
