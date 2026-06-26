using System.Collections.Generic;
using System.Linq;
using Gameplay.BattleEncounter.Characters.Behaviors;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class MultiActionBehaviorView : VisualElement
{
    private VisualElement _detail; 
    private string _searchText = "";
    private readonly List<VisualElement> _rows = new();


    public MultiActionBehaviorView()
    {
        style.flexGrow = 1;  
        var uss = AssetDatabase.LoadAssetAtPath<StyleSheet>(
            "Assets/CustomEditor/TableView/MultiActionBehavior/MultiActionBehaviorView.uss");
        VisualElement root = new VisualElement();
        if (uss != null) 
        {
            root.styleSheets.Add(uss);
        }
        var spilt = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
        spilt.style.flexGrow = 1;
        root.Add(spilt);
        root.style.flexGrow = 1;  
        spilt.Add(MakeSidebar());
        _detail = new VisualElement();
        _detail.style.flexGrow = 1;
        spilt.Add(_detail);        
        this.Add(root);
    }

    private VisualElement MakeSidebar()
    {
        _rows.Clear();
        var nav = new VisualElement();
        nav.AddToClassList("nav");
        nav.style.flexGrow = 1;
        
        var scroll = new ScrollView();
        scroll.style.flexGrow = 1;
        scroll.Add(MakeHeader());
        
        
        var items = AssetDatabase.FindAssets($"t:MultiActionBehavior")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<MultiActionBehavior>)
            .Where( item => item != null)
            .Select(item =>
            {
                var (cha, s) = ParseName(item.name);
                return (asset: item, character: cha, size: s);
            }).ToList();
        foreach (var group in items.GroupBy((x => x.character)).OrderBy(group => group.Key))
        {
            var foldout = new Foldout
            {
                text = ObjectNames.NicifyVariableName(group.Key), value = false
            };
            foldout.AddToClassList("nav-foldout");
            foreach (var item in group.OrderBy(x => VariantOrder(x.size)))
            {
                var label = new Label(item.asset.name);
                label.AddToClassList("nav-item");
                label.RegisterCallback<ClickEvent>(_ => ShowDetail(item.asset));
                foldout.Add(label);
            }
            scroll.Add(foldout);
            foldout.userData = group.Select(x => x.asset.name.ToLower()).ToArray();
            _rows.Add(foldout);
        }
        nav.Add(scroll);

        return nav;
    }

    private VisualElement MakeHeader()
    {
        var header = new VisualElement();
        header.AddToClassList("header");
        var label = new Label("MultiActionBehavior");
        int count = AssetDatabase.FindAssets($"t:MultiActionBehavior").Length;
        var subLabel = new Label($"{count} assets ground by character");
        subLabel.AddToClassList("subLabel");
        header.Add(label);
        header.Add(subLabel);
        var search = new ToolbarSearchField();
        search.RegisterValueChangedCallback(e =>
        {
            _searchText = e.newValue;
            ApplyFilter();
        });
        header.Add(search);
        return header;

    }
    private void ApplyFilter()
    {
        string words = (_searchText ?? "").ToLower();

        foreach (var foldout  in _rows)
        {
            var names  = foldout.userData as string[];
            
            bool matchSearch = words == "" || ((Foldout)foldout).text.ToLower().Contains(words) || (names != null && names.Any(n => n.Contains(words)));;
            foldout.style.display = (matchSearch) ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    private void ShowDetail(MultiActionBehavior itemAsset)
    {
        _detail.Clear();
        _detail.Add(new InspectorElement(itemAsset));
    }

    private int VariantOrder(string size)
    {
        switch (size)
        {
            case "S":
                return 0;
            case "M":
                return 1;
            case "L":
                return 2;
            default:
                return 99;
        }
    }

    private (string cha, string s) ParseName(string itemName)
    {
        var parts = itemName.Split('_');
        string character = parts.Length > 0 ? parts[0] : itemName;
        string size = parts.Length  > 1 ? parts[1] : "";
        return (character, size);
    }
}
