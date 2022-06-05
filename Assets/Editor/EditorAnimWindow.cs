using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

public class EditorAnimWindow : EditorWindow
{
   
    string FillButton = "FillList";
    string focus = "Focus";
    string AllAnimator = "ListAnims";
    string Play = "Play";
    float _lastEditorTime = 0f;
    float animationSpeed = 1;
    float animTime;
    

     Animator SelectedAnimator;
     AnimationClip SelectedClip;
    

    public List<Animator> ListAnimator = new List<Animator>();
    List<AnimationClip> PlayerAnimation = new List<AnimationClip>();
    List<bool> Player = new List<bool>();

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Editor/Animation")]
    static void Init()
    {
        EditorAnimWindow window = (EditorAnimWindow)EditorWindow.GetWindow(typeof(EditorAnimWindow));
        window.Show();
    }

    
    
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += _OnPlayModeStateChange;
        EditorSceneManager.activeSceneChangedInEditMode += OnSceneChange;
    }

    private void OnSceneChange(Scene arg1, Scene arg2) 
    {
        StopAnimSimulation();
    }

    private void _OnPlayModeStateChange(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            StopAnimSimulation();
        }
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= _OnPlayModeStateChange;
        StopAnimSimulation();
    }

    void OnGUI()
     {
        

        if (GUILayout.Button(FillButton))
        {
            Animator[] TempAnim = FindObjectsOfType<Animator>();
            foreach (Animator anim in TempAnim)
            {
                if (!ListAnimator.Contains(anim)) 
                {
                    ListAnimator.Add(anim);
                    Debug.Log(anim.runtimeAnimatorController.animationClips.Length);

                }

            }
            
        }
        for (int i = 0; i < ListAnimator.Count; i++)
        {
            EditorGUILayout.Space();
            bool PlayerButton = new bool();
            PlayerButton = false;
            Player.Add(PlayerButton);
            


            GUILayout.Label(ListAnimator[i].name, EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(focus))
            {
                Selection.activeGameObject = ListAnimator[i].gameObject;
            }
            if (GUILayout.Button(AllAnimator))
            {
                
                GUILayout.BeginHorizontal();
                AnimationClip[] TempAnim = ListAnimator[i].runtimeAnimatorController.animationClips;
                
                for (int v = 0; v < TempAnim.Length; v++)
                {
                    if (!PlayerAnimation.Contains(TempAnim[v]))
                    {
                        PlayerAnimation.Add(TempAnim[v]);
                    }
                    
                     
                }
                
                if (Player[i])
                {
                    Player[i] = false;
                    SelectedAnimator = null;
                    SelectedClip = null;
                }
                else
                {
                    Player[i] = true;
                    
                }
                
                GUILayout.EndHorizontal();

            }
            GUILayout.EndHorizontal();
            
            if (PlayerAnimation.Count != 0 && Player[i])
            {
                
                GUILayout.BeginHorizontal();
                for (int v = 0; v < PlayerAnimation.Count; v++)
                {
                     if(GUILayout.Button(PlayerAnimation[v].name))
                     {

                        SelectedAnimator = ListAnimator[i];
                        SelectedClip = PlayerAnimation[v];
                        
                        
                     }
                   
                }
                GUILayout.EndHorizontal();

                
                
            }
            
        }

        EditorGUILayout.Space();

        if (SelectedClip != null)
        {
            
            if (GUILayout.Button(Play))
            {
                StartAnimSimulation();
            }
            EditorGUILayout.Space();
            

            EditorGUILayout.Space();

            animTime = EditorGUILayout.Slider(animTime, 0, SelectedClip.length);

            animationSpeed = EditorGUILayout.FloatField("Animation Speed", animationSpeed);
           
           
        }
        
    }
    void OnInspectorUpdate()
    {
        if (SelectedClip != null)
        {
            SelectedClip.SampleAnimation(SelectedAnimator.gameObject, animTime);
            
            Debug.Log("inspector Updated");
        }
    }
    private void OnEditorUpdate()
    {
       
        animTime = (Time.realtimeSinceStartup - _lastEditorTime) * animationSpeed;
        if (SelectedClip!= null) 
        {
            if (animTime >= SelectedClip.length)
            {
                StopAnimSimulation();
                animTime = 0;
            }
            else
            {
                if (AnimationMode.InAnimationMode())
                {
                    AnimationMode.SampleAnimationClip(SelectedAnimator.gameObject, SelectedClip, animTime);
                }
            }

        }
        
    }

    

    public void StartAnimSimulation()
    {
        if (!Application.isPlaying)
        {
            
            AnimationMode.StartAnimationMode();
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.update += OnEditorUpdate;
            _lastEditorTime = Time.realtimeSinceStartup;
        }
        
        
        
    }

    public void StopAnimSimulation()
    {
        AnimationMode.StopAnimationMode();
        EditorApplication.update -= OnEditorUpdate;
       
    }

   
   
}
