The plan:

* Have a better check for IsNullable - that is, if the reference type can be `null`, which ... yes, every reference type can be assigned `null`, but if it's annotated with `?`, or if it's a value type that is really `Nullable<T>`.
* Only generate the case for null on the reader and writer sides if at least one property type is a reference type or a `Nullable<T>`. Otherwise, it's impossible to get `null` - i.e. the case where all property types are non-nullable value types.