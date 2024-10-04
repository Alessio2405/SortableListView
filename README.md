# SortableListView

Since I didn't find any FULLY working Listview handler with a Sort Adorner and full Sorting mechanism, I decided to write this little project to help everyone with this issue.

SortableListView is a WPF ListView component that extends the standard ListView control to provide full sorting functionality with visual feedback (sort adorners). It resolves common issues encountered when implementing sorting across multiple ListViews, particularly issues related to sorting adorners persisting or conflicting when switching between ListViews or columns.



## Features

Multi-ListView Sorting Support: Handles sorting across multiple ListViews in the same process, ensuring each ListView behaves independently.
Visual Sort Feedback: Displays visual indicators (sort adorners) to show which column is currently sorted and in which direction (ascending/descending).
Robust Adorner Management: Automatically clears existing adorners when switching between different ListViews or columns, preventing duplicate adorners.
Flexible Sorting: Easily customizable sorting logic using the property name or custom sorting fields defined in your GridViewColumn.
Support for Dynamic Data: Works seamlessly with ObservableCollection or any data-bound collections to handle dynamic data updates with consistent sorting behavior.
Problem Solved
Traditionally, handling sorting adorners in WPF's ListView can lead to issues when dealing with multiple ListViews, as adorners can overlap or remain after switching between different views. SortableListView addresses this by:



## Usage

### Prerequisites

- **Platform**: Windows operating system.
- **Development Environment**: Visual Studio or any other C# development environment.
- **Dependencies**: None other than standard .NET libraries.



### Instructions

Add the SortableListView component to your XAML file.
Bind your ListView to a data source.
Define your columns and specify the sort fields as needed.
Sorting will be automatically handled with visual feedback, ensuring proper behavior across all ListViews.

```bash
<ListView x:Name="myListView" ...>
    <ListView.View>
        <GridView>
            <GridViewColumn Header="Name" SortBy="Name" ... />
            <GridViewColumn Header="Age" SortBy="Age" ... />
        </GridView>
    </ListView.View>
</ListView>
Example
csharp
Copia codice
// C# Example of how to trigger sorting programmatically
myListView.SortGridViewColumnHeader(header, e);
```



## Author

- (https://github.com/Alessio2405)

Feel free to fork this project, modify it, and use it according to your needs!
