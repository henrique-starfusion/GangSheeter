# GangSheeter Application Development Plan

## Overview
This document outlines the development plan for the GangSheeter application, detailing the necessary components, functionalities, and interactions required to meet the specified requirements.

## Key Components

### 1. Image Management
- **Class:** ImageManager
  - **Methods:**
    - UploadImages(List<Image> images)
    - RemoveImage(Image image)
    - UpdateImageDetails(Image image, int copies, double width, double height, int dpi)

### 2. Printing Sheet Generation
- **Class:** PrintSheetGenerator
  - **Methods:**
    - GeneratePrintSheet(List<Image> images)
    - OptimizeLayout(List<Image> images)

### 3. Machine Learning Integration
- **Class:** LayoutOptimizer
  - **Methods:**
    - TrainModel(List<LayoutData> previousLayouts)
    - PredictLayout(List<Image> images)

### 4. User Interface
- **Class:** MainWindow (WPF)
  - **Components:**
    - Image upload button
    - Image list view with editable fields (copies, dimensions, DPI)
    - Generate button for print sheet
    - Reorganize button for layout optimization
    - Save button for exporting TIFF

### 5. Database Interaction
- **Class:** DatabaseContext (Entity Framework Core)
  - **Methods:**
    - SaveSettings(Settings settings)
    - LoadSettings()
    - SaveGeneratedSheet(SheetData sheetData)

### 6. Configuration Settings
- **Class:** Settings
  - **Properties:**
    - SheetWidth
    - MaxHeight
    - ExportDPI
    - CompressionType

## Functionalities
- Upload multiple images with interactive thumbnails.
- Generate a print sheet with a fixed width and dynamic height.
- Allow user interaction with the print sheet (move, rotate images).
- Export the final layout as a TIFF file with specified settings.
- Store and retrieve configuration settings and generated sheet history from SQLite.

## Follow-up Steps
- Implement the classes and methods as outlined.
- Test each functionality to ensure compliance with the requirements.
- Optimize the machine learning model based on user feedback and historical data.

## Conclusion
This plan serves as a roadmap for the development of the GangSheeter application, ensuring that all specified functionalities are addressed and implemented effectively.
