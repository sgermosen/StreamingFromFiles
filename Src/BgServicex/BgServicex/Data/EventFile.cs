﻿namespace BgServicex.Data
{
    public class EventFile : AuditEntity, IBaseEntity, ISoftDeleted
    {
        public int Id { get; set; }
        public string Attributes { get; set; } 
        public string FileName { get; set; }
        public string DirectoryName { get; set; }
        public string Size { get; set; }

    }
}