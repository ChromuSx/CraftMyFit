import shutil
import os
from pathlib import Path

def copy_files_from_subfolders():
    # Get the path of the directory where the script is located
    current_dir = Path(__file__).resolve().parent
    
    # Set the source directory to the script's location
    src_dir = current_dir
    
    # Define the destination directory inside the source directory
    dest_dir = src_dir / 'FilesAggregate'
    
    # Create the destination directory if it doesn't exist
    if not dest_dir.exists():
        dest_dir.mkdir()

    # Sets of directories and file extensions to exclude/include
    excluded_dirs = {'.git', '.vs', 'bin', 'obj', 'Debug', 'Release', 'packages', 'Migrations', 'SmartAttachments'}
    included_file_extensions = {'.xaml', '.xaml.cs', '.cs', '.html', '.cshtml', '.css', '.js', '.mrt', '.json'}

    # Walk through the source directory and its subfolders
    for root, dirs, files in os.walk(src_dir):
        # Skip directories that are in the excluded list
        dirs[:] = [d for d in dirs if d not in excluded_dirs]
        
        for file in files:
            # Get the full path of the file
            file_path = Path(root) / file
            
            # Ignore the script itself and the destination folder
            if file_path == Path(__file__) or dest_dir in file_path.parents:
                continue

            # Check if the file has an allowed extension
            if file_path.suffix in included_file_extensions:
                # Create the destination file path
                dest_file_path = dest_dir / file
                
                # Check if a file with the same name already exists in the destination
                if dest_file_path.exists():
                    # Rename the file by adding a number suffix
                    base = file_path.stem
                    extension = file_path.suffix
                    i = 1
                    while dest_file_path.exists():
                        dest_file_path = dest_dir / f"{base}_{i}{extension}"
                        i += 1
                
                # Copy the file to the destination folder
                shutil.copy2(file_path, dest_file_path)
                print(f"Copied: {file_path} -> {dest_file_path}")

# Run the function
copy_files_from_subfolders()