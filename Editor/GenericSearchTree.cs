/// <summary>
///* To create a search tree in the editor for a specific type and subtypes of a class
///* subtypes can be specified to use them as categories
///* similar version were used for the action system and the old BT system
///* this code can be used more like a template for specific search trees
/// </summary>

#if UNITY_EDITOR 

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Og.SmartFields
{
    public class GenericSearchTree : ScriptableObject, ISearchWindowProvider
    {
        private Action<Type> onSelectType;

        Type mainType;
        List<Type> categories;
        List<string> negativeWords = new List<string>(); //* words to remove form class names for cleaner display
        public void Initialize(Action<Type> onSelectCallback)
        {
            onSelectType = onSelectCallback;
        }

        public void InitCategories(Type _mainType, List<Type> _categories)
        {
            mainType = _mainType;
            categories = _categories;
        }

        public void AddNegativeWords(string word)
        {
            negativeWords.Add(word);
        }

        public static List<Type> GetAllTypesDerived(Type category)
        {
            var types = new List<Type>();

            //* Include the base type itself if it is not abstract
            if (!category.IsAbstract)
            {
                types.Add(category);
            }

            //* Get all derived types
            types.AddRange(TypeCache.GetTypesDerivedFrom(category));

            return types;
        }
        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            onSelectType?.Invoke(searchTreeEntry.userData as Type);
            return true;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Search"), 0)
            };

            var allTypes = GetAllTypesDerived(mainType);
            var usedTypes = new HashSet<Type>();

            foreach (Type category in categories)
            {
                searchTree.Add(new SearchTreeGroupEntry(new GUIContent(GetDisplayName(category)), 1)); // Use category.Name for cleaner display
                var categoryTypes = GetAllTypesDerived(category);

                categoryTypes.RemoveAll(t => usedTypes.Contains(t)); // Remove types already used
                usedTypes.UnionWith(categoryTypes);
                foreach (var categoryType in categoryTypes)
                {
                    searchTree.Add(new SearchTreeEntry(new GUIContent(GetDisplayName(categoryType))) // Directly add types under the category
                    {
                        level = 2,
                        userData = categoryType
                    });
                }
            }

            allTypes.RemoveAll(t => usedTypes.Contains(t)); // Remove types already used
            foreach (var type in allTypes)
            {
                searchTree.Add(new SearchTreeEntry(new GUIContent(GetDisplayName(type))) // Add remaining types at the root level
                {
                    level = 1,
                    userData = type
                });
            }

            return searchTree;
        }

        public string GetDisplayName(Type type)
        {
            string name = type.Name;
            foreach (string word in negativeWords)
            {
                name = name.Replace(word, "");
            }
            return name;
        }
    }
}
#endif