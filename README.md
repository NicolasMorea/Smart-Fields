# Smart-Fields

Custom Unity Editor fields that let you apply conditional logic (filtering and conditional based serialization) to variables in the Unity Inspector using attributes.

# Attributes

Here’s a breakdown of the custom attributes in this repo and what each one is used for:

---

#### Optional  
Adds an **Enabled** toggle next to the value, allowing optional behavior. The field is greyed if not used. (The property still takes memory)

```cs
public Optional<bool> optionalBool;

void Start() => if(optionalBool.Enabled) print($"My bool is {optionalBool.Value});
```

#### Enum Dependent Field
Shows a field only when a referenced enum matches a specific value.

```cs
[SerializeField] private MyEnum myEnum;

[EnumDependentField(nameof(myEnum), typeof(MyEnum), (int)MyEnum.FisrtValue)] public bool showIfMyEnumIsValueOne;
[EnumDependentField(nameof(myEnum), typeof(MyEnum), (int)MyEnum.TwoValue)] public bool showIfMyEnumIsValueTwo;
```

#### One Line Property

Displays all fields of a serialized class on a single horizontal line in the inspector.
```cs
[OneLineProperty] public MyClass myClass;
```

#### Type Pick

Allows selecting a subclass type for a SerializeReference field directly in the inspector.
```cs
public abstract class EntityData {}

public class EnemyData : EntityData {}

public class PlayerData : PlayerData {}

public class DataPicker
{
    [SerializeReference, TypePick] private DirectAttackData attackData;
}
```

## Other

#### Generic Search Tree
The Generic Search Tree allows to generate a search tree for inherited class from a source class in the editor, negative words can be used to simplify the names (inherited types often include some common words in the name)
