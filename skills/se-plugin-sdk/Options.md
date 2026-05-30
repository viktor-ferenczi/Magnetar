# Option Attributes

Every config property carries exactly one `*OptionAttribute` from
`PluginSdk.Config`. The attribute pairs a property with its value type and
carries the metadata the UI needs (description, range, length, etc.).

All option attributes accept an optional `Description` string (the tooltip /
help text shown next to the field) and an optional `Parent = "<container-id>"`
to attach the option to a layout container — see [Layout.md](Layout.md).

## Scalars

| Attribute | Property type | Extra params |
|---|---|---|
| `[BoolOption]` | `bool` | — |
| `[IntOption(min, max)]` | `int` | inclusive `min`/`max`, both optional |
| `[LongOption(min, max)]` | `long` | inclusive `min`/`max`, both optional |
| `[FloatOption(min, max)]` | `float` | inclusive `min`/`max`, both optional |
| `[DoubleOption(min, max)]` | `double` | inclusive `min`/`max`, both optional |
| `[StringOption(maxLength, pattern)]` | `string` | `maxLength = 0` means unlimited; `pattern` is an optional full-match regex |

Numeric ranges left unspecified are emitted as **absent** in the schema (not
as the type's `MinValue`/`MaxValue` sentinel), so Quasar can render an
unbounded editor.

Examples:

```csharp
[BoolOption("Enable verbose logging")]
public bool Verbose { get => verbose; set => SetField(ref verbose, value); }

[IntOption(1, 240, "Ticks per second")]
public int TickRate { ... }

[StringOption(maxLength: 64, pattern: @"^[A-Za-z0-9_-]+$", description: "Slug")]
public string Slug { ... }
```

## Lists

`[ListOption(maxCount, description)]` marks a `List<T>` where `T` is a scalar
or a user struct. `maxCount = 0` (the default) means unlimited.

```csharp
[ListOption(description: "Whitelisted ports")]
public List<int> Ports { get => ports; set => SetField(ref ports, value); }

[ListOption(maxCount: 100, description: "Per-player overrides")]
public List<PlayerOverride> Overrides { ... }
```

### Tree-shaped lists

If the element type is a struct that carries a parent reference, set
`TreeParentField` to the name of that struct member. Quasar then renders the
list as a tree:

```csharp
public struct Node
{
    [StructMember] public int Id;
    [StructMember] public int ParentId;     // <-- pointed at by TreeParentField
    [StructMember] public string Label { get; set; }
}

[ListOption(description: "Menu tree", TreeParentField = nameof(Node.ParentId))]
public List<Node> Menu { ... }
```

The plugin still gets a flat `List<Node>`; the tree shape is purely a UI hint.

## Dictionaries

`[DictOption(maxCount, description)]` marks a
`SerializableDictionary<TKey, TValue>` (see [Config.md](Config.md#supported-property-types)).

- `TKey`: `string`, `int`, or `long`.
- `TValue`: any scalar, or a user struct (set `TreeParentField` to a struct
  member referencing another entry's key for a tree-rendered dictionary).

```csharp
[DictOption(description: "Per-player quotas")]
public SerializableDictionary<string, int> Quotas { ... }
```

## Struct options

`[StructOption(description)]` marks either a single struct value or a
`List<TStruct>`. The struct's public fields and properties must each carry
`[StructMember(description)]` to be included in the schema and to be visible
to Quasar.

```csharp
public struct Range
{
    [StructMember("Lower bound")]  public int Min;
    [StructMember("Upper bound")]  public int Max;
}

[StructOption(description: "Allowed range")]
public Range Bounds { get => bounds; set => SetField(ref bounds, value); }

[StructOption(description: "Tiered ranges")]
public List<Range> Tiers { get => tiers; set => SetField(ref tiers, value); }
```

A struct may also be used as the element type of a `List<T>` via the plain
`[ListOption]`, or as the value type of a dictionary via `[DictOption]` —
`[StructOption]` is only required when the property itself *is* the struct (or
a list of structs) and is convenient for clarity.

### Struct member rules

- `[StructMember]` is **mandatory** on each public field/property you want in
  the schema. Members without it are ignored.
- Property members must be both readable and writable (`{ get; set; }`).
- Member types follow the same catalogue as top-level options: scalars,
  `List<T>`, `SerializableDictionary<,>`, or nested structs.
- A struct may be a `List<T>` element, but **not** a dictionary key.

### Constraints inside structs

There is intentionally no `IntStructMember(min, max)` or similar. Struct
members carry only a description. If you need range-validated values, put the
field on the config class directly with the appropriate option attribute
instead of nesting it in a struct.

## Choosing list vs. dict vs. struct

- **Same shape, ordered, identity is positional** → `List<scalar>` or
  `List<struct>`.
- **Lookup by name/id** → `SerializableDictionary<string, …>`.
- **Fixed-arity record with named fields** → struct with `[StructOption]`.
- **Hierarchy a user edits as a tree** → `List<struct>` or
  `SerializableDictionary<,struct>` plus `TreeParentField`.
