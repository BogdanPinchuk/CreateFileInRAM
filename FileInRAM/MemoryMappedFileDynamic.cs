using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace FileInRAM
{
    /// <summary>
    /// Dynamic MemoryMappedFileDynamic
    /// </summary>
    public class MemoryMappedFileDynamic : IDisposable
    {
        enum CreateInstance
        {
            CreateFromFile,
            CreateNew,
            CreateOrOpen,
            OpenExistingstring,
        }
        enum CreateAccess
        {
            CreateViewAccessor,
            CreateViewStream,
        }

        private MemoryMappedFile instance;
        private MemoryMappedViewStream stream;
        private MemoryMappedViewAccessor accessor;

        private FileMode mode;
        private bool leaveOpen;
        private string path, mapName;
        private FileStream fileStream;
        private long capacity, offset, size;
        private MemoryMappedFileAccess access;
        private MemoryMappedFileOptions options;
        private HandleInheritability inheritability;
        private MemoryMappedFileRights desiredAccessRights;

        private int ni, na;
        private CreateInstance ci;
        private CreateAccess ca;

        public MemoryMappedFile Instance
        {
            get { return instance; }
        }
        public MemoryMappedViewStream Stream
        {
            get { return stream; }
            set
            {
                if (stream.Length < value.Length)
                {
                    MemoryMappedFile instance = MemoryMappedFile.CreateNew(string.Empty, 1);
                    capacity = value.Length;
                    size = capacity;

                    switch (ci)
                    {
                        case CreateInstance.CreateFromFile:
                            #region
                            if (ni <= 3)
                                instance = MemoryMappedFile.CreateFromFile(path, mode, mapName, capacity);
                            else if (ni == 4)
                                instance = MemoryMappedFile.CreateFromFile(path, mode, mapName, capacity, access);
                            else if (ni == 5)
                                instance = MemoryMappedFile.CreateFromFile(fileStream, mapName, capacity, access, inheritability, leaveOpen);
                            break;
                        #endregion
                        case CreateInstance.CreateNew:
                            #region
                            if (ni == 0)
                                instance = MemoryMappedFile.CreateNew(mapName, capacity);
                            else if (ni == 1)
                                instance = MemoryMappedFile.CreateNew(mapName, capacity, access);
                            else if (ni == 2)
                                instance = MemoryMappedFile.CreateNew(mapName, capacity, access, options, inheritability);
                            break; 
                        #endregion
                        case CreateInstance.CreateOrOpen:
                            #region
                            if (ni == 0)
                                instance = MemoryMappedFile.CreateOrOpen(mapName, capacity);
                            else if (ni == 1)
                                instance = MemoryMappedFile.CreateOrOpen(mapName, capacity, access);
                            else if (ni == 2)
                                instance = MemoryMappedFile.CreateOrOpen(mapName, capacity, access, options, inheritability);
                            break;
                            #endregion
                            #region hold
#if false
                        case CreateInstance.OpenExistingstring:
                            #region
                            if (ni == 0)
                                instance = MemoryMappedFile.CreateOrOpen(mapName, capacity);
                            else if (ni == 1)
                                instance = MemoryMappedFile.CreateOrOpen(mapName, capacity, access);
                            else if (ni == 2)
                                instance = MemoryMappedFile.CreateOrOpen(mapName, capacity, access, options, inheritability);
                            break;
                            #endregion
#endif 
                            #endregion
                    }

                    MemoryMappedViewStream stream = instance.CreateViewStream(1, 1);
                    if (na <= 1)
                        stream = instance.CreateViewStream(offset, size);
                    else if (na == 2)
                        stream = instance.CreateViewStream(offset, size, access);

                    this.stream.CopyTo(stream);
                    this.stream.Flush();
                    this.stream.Close();

                    this.instance.Dispose();
                    this.instance = instance;
                    stream.CopyTo(this.stream);

                    instance.Dispose();
                    stream.Dispose();
                }
                else
                {
                    stream = value;
                }
            }
        }
        public MemoryMappedViewAccessor Accessor
        {
            get { return accessor; }
        }

        private MemoryMappedFileDynamic(MemoryMappedFile instance)
            => this.instance = instance;


        public void CreateFromFile(string path)
        {
            instance = MemoryMappedFile.CreateFromFile(path);
            this.path = path;
            ci = CreateInstance.CreateFromFile;
            ni = 0;
        }
        public void CreateFromFile(string path, FileMode mode)
        {
            instance = MemoryMappedFile.CreateFromFile(path, mode);
            this.path = path;
            this.mode = mode;
            ci = CreateInstance.CreateFromFile;
            ni = 1;
        }
        public void CreateFromFile(string path, FileMode mode, string mapName)
        {
            instance = MemoryMappedFile.CreateFromFile(path, mode, mapName);
            this.path = path;
            this.mode = mode;
            this.mapName = mapName;
            ci = CreateInstance.CreateFromFile;
            ni = 2;
        }
        public void CreateFromFile(string path, FileMode mode, string mapName, long capacity)
        {
            instance = MemoryMappedFile.CreateFromFile(path, mode, mapName, capacity);
            this.path = path;
            this.mode = mode;
            this.mapName = mapName;
            this.capacity = capacity;
            ci = CreateInstance.CreateFromFile;
            ni = 3;
        }
        public void CreateFromFile(string path, FileMode mode, string mapName, long capacity, MemoryMappedFileAccess access)
        {
            instance = MemoryMappedFile.CreateFromFile(path, mode, mapName, capacity, access);
            this.path = path;
            this.mode = mode;
            this.mapName = mapName;
            this.capacity = capacity;
            this.access = access;
            ci = CreateInstance.CreateFromFile;
            ni = 4;
        }
        public void CreateFromFile(FileStream fileStream, string mapName, long capacity, MemoryMappedFileAccess access, HandleInheritability inheritability, bool leaveOpen)
        {
            instance = MemoryMappedFile.CreateFromFile(fileStream, mapName, capacity, access, inheritability, leaveOpen);
            this.fileStream = fileStream;
            this.mapName = mapName;
            this.capacity = capacity;
            this.inheritability = inheritability;
            this.leaveOpen = leaveOpen;
            ci = CreateInstance.CreateFromFile;
            ni = 5;
        }

        public void CreateNew(string mapName, long capacity)
        {
            instance = MemoryMappedFile.CreateNew(mapName, capacity);
            this.mapName = mapName;
            this.capacity = capacity;
            ci = CreateInstance.CreateNew;
            ni = 0;
        }
        public void CreateNew(string mapName, long capacity, MemoryMappedFileAccess access)
        {
            instance = MemoryMappedFile.CreateNew(mapName, capacity, access);
            this.mapName = mapName;
            this.capacity = capacity;
            this.access = access;
            ci = CreateInstance.CreateNew;
            ni = 1;
        }
        public void CreateNew(string mapName, long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, HandleInheritability inheritability)
        {
            instance = MemoryMappedFile.CreateNew(mapName, capacity, access, options, inheritability);
            this.mapName = mapName;
            this.capacity = capacity;
            this.access = access;
            this.options = options;
            this.inheritability = inheritability;
            ci = CreateInstance.CreateNew;
            ni = 2;
        }

        public void CreateOrOpen(string mapName, long capacity)
        {
            instance = MemoryMappedFile.CreateOrOpen(mapName, capacity);
            this.mapName = mapName;
            this.capacity = capacity;
            ci = CreateInstance.CreateOrOpen;
            ni = 0;
        }
        public void CreateOrOpen(string mapName, long capacity, MemoryMappedFileAccess access)
        {
            instance = MemoryMappedFile.CreateOrOpen(mapName, capacity, access);
            this.mapName = mapName;
            this.capacity = capacity;
            this.access = access;
            ci = CreateInstance.CreateOrOpen;
            ni = 1;
        }
        public void CreateOrOpen(string mapName, long capacity, MemoryMappedFileAccess access, MemoryMappedFileOptions options, HandleInheritability inheritability)
        {
            instance = MemoryMappedFile.CreateOrOpen(mapName, capacity, access, options, inheritability);
            this.mapName = mapName;
            this.capacity = capacity;
            this.access = access;
            this.options = options;
            this.inheritability = inheritability;
            ci = CreateInstance.CreateOrOpen;
            ni = 2;
        }

        public void OpenExistingstring(string mapName)
        {
            instance = MemoryMappedFile.OpenExisting(mapName);
            this.mapName = mapName;
            ci = CreateInstance.OpenExistingstring;
            ni = 0;
        }
        public void OpenExistingstring(string mapName, MemoryMappedFileRights desiredAccessRights)
        {
            instance = MemoryMappedFile.OpenExisting(mapName, desiredAccessRights);
            this.mapName = mapName;
            this.desiredAccessRights = desiredAccessRights;
            ci = CreateInstance.OpenExistingstring;
            ni = 1;
        }
        public void OpenExistingstring(string mapName, MemoryMappedFileRights desiredAccessRights, HandleInheritability inheritability)
        {
            instance = MemoryMappedFile.OpenExisting(mapName, desiredAccessRights, inheritability);
            this.mapName = mapName;
            this.desiredAccessRights = desiredAccessRights;
            this.inheritability = inheritability;
            ci = CreateInstance.OpenExistingstring;
            ni = 2;
        }

        public void CreateViewAccessor()
        {
            accessor = instance.CreateViewAccessor();
            ca = CreateAccess.CreateViewAccessor;
            na = 0;
        }
        public void CreateViewAccessor(long offset, long size)
        {
            accessor = instance.CreateViewAccessor(offset, size);
            this.offset = offset;
            this.size = size;
            ca = CreateAccess.CreateViewAccessor;
            na = 1;
        }
        public void CreateViewAccessor(long offset, long size, MemoryMappedFileAccess access)
        {
            accessor = instance.CreateViewAccessor(offset, size, access);
            this.offset = offset;
            this.size = size;
            this.access = access;
            ca = CreateAccess.CreateViewAccessor;
            na = 2;
        }
        public void CreateViewStream()
        {
            stream = instance.CreateViewStream();
            ca = CreateAccess.CreateViewStream;
            na = 0;
        }
        public void CreateViewStream(long offset, long size)
        {
            stream = instance.CreateViewStream(offset, size);
            this.offset = offset;
            this.size = size;
            ca = CreateAccess.CreateViewStream;
            na = 1;
        }
        public void CreateViewStream(long offset, long size, MemoryMappedFileAccess access)
        {
            stream = instance.CreateViewStream(offset, size, access);
            this.offset = offset;
            this.size = size;
            this.access = access;
            ca = CreateAccess.CreateViewStream;
            na = 2;
        }

        public void Dispose()
            => instance.Dispose();

    }
}
