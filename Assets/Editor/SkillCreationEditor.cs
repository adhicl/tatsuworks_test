using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SkillCreationEditor : EditorWindow
{
    private ListView skillList;
    
    private TextField skillNameField;
    private TextField skillDescriptionField;
    private DoubleField skillCostField;
    private DoubleField skillCooldownField;

    private DoubleField skillDamageField;
    private EnumField skillDamageTypeField;
    private EnumField skillRangeTypeField;

    private Toggle scaleStrengthToggle;
    private Toggle scaleMagicToggle;
    private Toggle poisonToggle;
    private Toggle stunToggle;
    private Toggle slowToggle;

    private Slider scaleStrengthSlider;
    private Slider scaleMagicSlider;
    private DoubleField poisonDamageField;
    private DoubleField stunLengthField;
    private DoubleField slowLengthField;

    private ObjectField iconField;
    private ObjectField prefabField;
    
    private Button saveButton;
    private Button testButton;
    private Button newButton;

    [MenuItem("Tools/SkillCreationEditor")]
    public static void OpenEditorWindow()
    {
        SkillCreationEditor window = GetWindow<SkillCreationEditor>();
        window.titleContent = new GUIContent("Skill Creation Editor");
        window.maxSize = new Vector2(700, 500);
        window.minSize = window.maxSize;
    }

    private void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Settings/UI/SkillEditor.uxml");
        VisualElement tree = visualTree.Instantiate();
        root.Add(tree);
        
        //Assign Elements
        skillList = root.Q<ListView>("skillList");
        skillNameField = root.Q<TextField>("skillName");
        skillDescriptionField = root.Q<TextField>("skillDescription");
        skillCostField = root.Q<DoubleField>("skillManaCost");
        skillCooldownField = root.Q<DoubleField>("skillCooldown");
        
        skillDamageField = root.Q<DoubleField>("skillDamage");
        skillDamageTypeField = root.Q<EnumField>("skillDamageType");
        skillRangeTypeField = root.Q<EnumField>("skillRangeType");
        
        scaleStrengthToggle = root.Q<Toggle>("toggleScaleStrength");
        scaleStrengthSlider = root.Q<Slider>("sliderScaleStrength");
        scaleMagicToggle = root.Q<Toggle>("toggleScaleMagic");
        scaleMagicSlider = root.Q<Slider>("sliderScaleMagic");
        poisonToggle = root.Q<Toggle>("togglePoison");
        poisonDamageField = root.Q<DoubleField>("poisonDamage");
        slowToggle = root.Q<Toggle>("toggleSlow");
        slowLengthField = root.Q<DoubleField>("slowHold");
        stunToggle = root.Q<Toggle>("toggleStun");
        stunLengthField = root.Q<DoubleField>("stunHold");
        
        iconField = root.Q<UnityEditor.UIElements.ObjectField>("iconField");
        prefabField = root.Q<ObjectField>("prefabField");
        
        saveButton = root.Q<Button>("btnSave");
        saveButton.SetEnabled(false);
        
        testButton = root.Q<Button>("btnTest");
        testButton.SetEnabled(false);
        
        newButton = root.Q<Button>("btnCreate");

        //AssignCallBacks
        skillNameField.RegisterValueChangedCallback(NameFieldChanged);
        scaleStrengthToggle.RegisterValueChangedCallback(toggleScaleStrength);
        scaleMagicToggle.RegisterValueChangedCallback(toggleScaleMagic);
        slowToggle.RegisterValueChangedCallback(toggleSlow);
        stunToggle.RegisterValueChangedCallback(toggleStun);
        poisonToggle.RegisterValueChangedCallback(togglePoison);
        
        saveButton.RegisterCallback<ClickEvent>(SaveSkill);
        testButton.RegisterCallback<ClickEvent>(TestSkill);
        newButton.RegisterCallback<ClickEvent>(CreateNewSkill);

        LoadSkillList();
        refreshTalents();
    }
    
    //if there is name then can save skill
    private void NameFieldChanged(ChangeEvent<string> evt)
    {
        if (skillNameField.value.Trim().Length > 0)
        {
            saveButton.SetEnabled(true);
        }
        else
        {
            saveButton.SetEnabled(false);
        }
    }

    //load available skills in Assets/Skills
    private void LoadSkillList()
    {
        var assets = AssetDatabase.FindAssets($"t:{typeof(SkillConfig).Name}",new [] {"Assets/Skills"});
        
        skillList.itemsSource = assets;
        skillList.makeItem = () => new Label();
        skillList.bindItem = (VisualElement element, int index) =>
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(assets[index]);
            //Debug.Log($"Index {index} {AssetDatabase.LoadAssetAtPath<SkillConfig>(assetPath).name}");
            (element as Label).text = AssetDatabase.LoadAssetAtPath<SkillConfig>(assetPath).name;
        };
        skillList.selectionChanged += PickSkill;
        skillList.Rebuild();
    }

    private void refreshTalents()
    {
        if (scaleStrengthToggle.value)
        {
            scaleStrengthSlider.RemoveFromClassList("turn-off");
        }
        else
        {
            scaleStrengthSlider.AddToClassList("turn-off");
        }
        
        if (scaleMagicToggle.value)
        {
            scaleMagicSlider.RemoveFromClassList("turn-off");
        }
        else
        {
            scaleMagicSlider.AddToClassList("turn-off");
        }

        if (poisonToggle.value)
        {
            poisonDamageField.RemoveFromClassList("turn-off");
        }
        else
        {
            poisonDamageField.AddToClassList("turn-off");
        }

        if (stunToggle.value)
        {
            stunLengthField.RemoveFromClassList("turn-off");
        }
        else
        {
            stunLengthField.AddToClassList("turn-off");
        }

        if (slowToggle.value)
        {
            slowLengthField.RemoveFromClassList("turn-off");
        }
        else
        {
            slowLengthField.AddToClassList("turn-off");
        }
    }

    //===============================================call back methods===================================================
    private void PickSkill(IEnumerable<object> skillSelected)
    {
        SkillConfig skill = AssetDatabase.LoadAssetAtPath<SkillConfig>(AssetDatabase.GUIDToAssetPath(skillSelected.First().ToString()));
        skillNameField.value = skill.skillName;
        skillDescriptionField.value = skill.description;
        skillCostField.value = skill.manaCost;
        skillCooldownField.value = skill.cooldown;

        skillDamageField.value = skill.damage;
        skillDamageTypeField.value = skill.damageType;
        skillRangeTypeField.value = skill.rangeType;

        //reset talents
        scaleStrengthToggle.value = false;
        scaleStrengthSlider.value = 10;
        scaleMagicToggle.value = false;
        scaleMagicSlider.value = 10;
        poisonToggle.value = false;
        poisonDamageField.value = 0;
        slowToggle.value = false;
        slowLengthField.value = 0;
        stunToggle.value = false;
        stunLengthField.value = 0;
        
        for (int i = 0; i < skill.talents.Length; i++)
        {
            switch (skill.talents[i].type)
            {
                case TalentType.ScaleStrength:
                    scaleStrengthToggle.value = true;
                    scaleStrengthSlider.value = (float) skill.talents[i].value;
                    break;
                case TalentType.ScaleMagic:
                    scaleMagicToggle.value = true;
                    scaleMagicSlider.value = (float) skill.talents[i].value;
                    break;
                case TalentType.Poison:
                    poisonToggle.value = true;
                    poisonDamageField.value = skill.talents[i].value;
                    break;
                case TalentType.Stun:
                    stunToggle.value = true;
                    stunLengthField.value = skill.talents[i].value;
                    break;
                case TalentType.Slow:
                    slowToggle.value = true;
                    slowLengthField.value = skill.talents[i].value;
                    break;
            }
        }

        iconField.value = skill.iconAbility;
        prefabField.value = skill.prefabAbility;

        testButton.SetEnabled(true);
    }

    private void toggleScaleStrength(ChangeEvent<bool> evt)
    {
        scaleStrengthToggle.value = evt.newValue;
        refreshTalents();
    }
    
    private void toggleScaleMagic(ChangeEvent<bool> evt)
    {
        scaleMagicToggle.value = evt.newValue;
        refreshTalents();
    }

    private void togglePoison(ChangeEvent<bool> evt)
    {
        poisonToggle.value = evt.newValue;
        refreshTalents();
    }

    private void toggleSlow(ChangeEvent<bool> evt)
    {
        slowToggle.value = evt.newValue;
        refreshTalents();
    }

    private void toggleStun(ChangeEvent<bool> evt)
    {
        stunToggle.value = evt.newValue;
        refreshTalents();
    }

    private void SaveSkill(ClickEvent evt)
    {
        var assets = AssetDatabase.FindAssets($"{skillNameField.value}", new[] { "Assets/Skills" });
        
        if (assets.Length > 0)
        {
            //update skill config
            SkillConfig skillConfig = AssetDatabase.LoadAssetAtPath<SkillConfig>(AssetDatabase.GUIDToAssetPath(assets[0]));
            skillConfig.description = skillDescriptionField.value;
            skillConfig.skillName = skillNameField.value;
            skillConfig.cooldown = skillCooldownField.value;
            skillConfig.manaCost = skillCostField.value;
            skillConfig.damage = skillDamageField.value;
            skillConfig.damageType = (DamageType) skillDamageTypeField.value;
            skillConfig.rangeType = (RangeType) skillRangeTypeField.value;
            
            List<Talents> list = new List<Talents>();
            if (scaleStrengthToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.ScaleStrength;
                talents.value = scaleStrengthSlider.value;
                list.Add(talents);
            }
            if (scaleMagicToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.ScaleMagic;
                talents.value = scaleMagicSlider.value;
                list.Add(talents);
            }
            if (poisonToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.Poison;
                talents.value = poisonDamageField.value;
                list.Add(talents);
            }
            if (stunToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.Stun;
                talents.value = stunLengthField.value;
                list.Add(talents);
            }
            if (slowToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.Slow;
                talents.value = slowLengthField.value;
                list.Add(talents);
            }

            skillConfig.talents = new Talents[list.Count];
            for(var i = 0; i < skillConfig.talents.Length; i++)
            {
                skillConfig.talents[i] = list[i];
            }
            
            skillConfig.iconAbility = (Sprite) iconField.value;
            skillConfig.prefabAbility = (GameObject) prefabField.value;
        }
        else
        {
            //create new skill config
            SkillConfig skillConfig = ScriptableObject.CreateInstance<SkillConfig>();
            skillConfig.skillName = skillNameField.value;
            skillConfig.description = skillDescriptionField.value;
            skillConfig.description = skillDescriptionField.value;
            skillConfig.skillName = skillNameField.value;
            skillConfig.cooldown = skillCooldownField.value;
            skillConfig.manaCost = skillCostField.value;
            skillConfig.damage = skillDamageField.value;
            skillConfig.damageType = (DamageType) skillDamageTypeField.value;
            skillConfig.rangeType = (RangeType) skillRangeTypeField.value;
            
            List<Talents> list = new List<Talents>();
            if (scaleStrengthToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.ScaleStrength;
                talents.value = scaleStrengthSlider.value;
                list.Add(talents);
            }
            if (scaleMagicToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.ScaleMagic;
                talents.value = scaleMagicSlider.value;
                list.Add(talents);
            }
            if (poisonToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.Poison;
                talents.value = poisonDamageField.value;
                list.Add(talents);
            }
            if (stunToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.Stun;
                talents.value = stunLengthField.value;
                list.Add(talents);
            }
            if (slowToggle.value)
            {
                Talents talents = new Talents();
                talents.type = TalentType.Slow;
                talents.value = slowLengthField.value;
                list.Add(talents);
            }

            skillConfig.talents = new Talents[list.Count];
            for(var i = 0; i < skillConfig.talents.Length; i++)
            {
                skillConfig.talents[i] = list[i];
            }

            skillConfig.iconAbility = (Sprite) iconField.value;
            skillConfig.prefabAbility = (GameObject) prefabField.value;
            
            AssetDatabase.CreateAsset(skillConfig, $"Assets/Skills/{skillNameField.value}.asset");
            testButton.SetEnabled(true);
        }
        LoadSkillList();
    }

    private void CreateNewSkill(ClickEvent evt)
    {
        skillNameField.value = "";
        skillDescriptionField.value = "";
        skillCooldownField.value = 0;
        skillCostField.value = 0;
        skillDamageField.value = 0;
        skillDamageTypeField.value = DamageType.Physical;
        skillRangeTypeField.value = RangeType.Single;
        
        scaleMagicToggle.value = false;
        scaleStrengthToggle.value = false;
        scaleMagicSlider.value = 10;
        scaleStrengthSlider.value = 10;
        poisonToggle.value = false;
        stunToggle.value = false;
        slowToggle.value = false;
        poisonDamageField.value = 0;
        stunLengthField.value = 0;
        slowLengthField.value = 0; 
        
        iconField.value = null;
        prefabField.value = null;

        testButton.SetEnabled(false);
        saveButton.SetEnabled(false);
        skillList.ClearSelection();
    }

    private void TestSkill(ClickEvent evt)
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "TestSkillScene")
        {
            Debug.LogError("Open the TestSkillScene and press play");
            EditorSceneManager.LoadSceneInPlayMode("TestSkillScene", new LoadSceneParameters());
        }
        
        if (!EditorApplication.isPlaying)
        {
            var assets = AssetDatabase.FindAssets($"{skillNameField.value}", new[] { "Assets/Skills" });

            if (assets.Length > 0)
            {
                EditorApplication.EnterPlaymode();
            }
        }
        else
        {
            var assets = AssetDatabase.FindAssets($"{skillNameField.value}", new[] { "Assets/Skills" });

            if (assets.Length > 0)
            {
                SkillConfig skillConfig =
                    AssetDatabase.LoadAssetAtPath<SkillConfig>(AssetDatabase.GUIDToAssetPath(assets[0]));
                GameController.Instance.SetAbility(skillConfig);
            }
        }
    }
}
