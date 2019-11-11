# CustomFieldsComponent

This plugin adds a `CustomFieldsComponent` and changes the `FindEntityPipeline` and the `PersistEntityPipeline` to update the `ExternalSettingsComponent`.

Usage:
```
    var customFieldsComponent = sellableItem.GetComponent<CustomFieldsComponent>();
    customFieldsComponent.CustomFields.Add("MyCustomField", myCustomField);
```

The values in the CustomFieldsComponent will be copied to the shared settings on the ExternalSettingsComponent. 

In Sitecore, create a template that contains *Shared* fields so these can be edited in the Content editor. 
