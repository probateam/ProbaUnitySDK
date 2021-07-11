using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Proba;
using UnityEditor;
using UnityEngine;

namespace Proba.Editor
{
    internal class EventRecorder : EditorWindow
    {
        #region variables

        private Vector2 scrollPos;
        private List<EventDataScheme> _dataSchemes;

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
            Application.OpenURL("https://github.com/probateam/ProbaUnitySDK");
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
                DatabaseConnection.InitialDatabaseConnection();
                _dataSchemes = DatabaseConnection.GetAllEvents();
            }
            if (GUILayout.Button("Flush Database"))
            {
                DatabaseConnection.InitialDatabaseConnection();
                DatabaseConnection.FlushDatabase();
                _dataSchemes = DatabaseConnection.GetAllEvents();
            }
            EditorGUILayout.EndHorizontal();


            if (_dataSchemes == null)
            {
                EditorGUILayout.LabelField("Press 'Get Event List'");
                return;
            }
            var i = 0;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (var dataScheme in _dataSchemes)
            {
                var id = dataScheme.ID;
                var body = GetEventBody(dataScheme.BODY, out var style);
                var date = dataScheme.DATE;
                var texture = MakeTex(200, 200, GetColor(i++));

                EditorGUILayout.BeginVertical(style);
                GUILayout.Label(id + " " + body + " " + date);
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
