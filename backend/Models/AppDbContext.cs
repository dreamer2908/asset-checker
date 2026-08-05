using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LegacyWebBridge.Models;

public partial class AppDbContext : DbContext
{
    private readonly IConfiguration? _configuration;

    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration? configuration = null)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Astmb> Astmbs { get; set; }

    public virtual DbSet<Astmc> Astmcs { get; set; }

    public virtual DbSet<Cmsme> Cmsmes { get; set; }

    public virtual DbSet<Cmsmv> Cmsmvs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string? connectionString = _configuration?.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                var basePath = Directory.GetCurrentDirectory();
                var config = new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();

                connectionString = config.GetConnectionString("DefaultConnection");
            }

            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Astmb>(entity =>
        {
            entity.HasKey(e => e.Mb001);

            entity.ToTable("ASTMB");

            entity.Property(e => e.Mb001)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("MB001");
            entity.Property(e => e.Company)
                .HasMaxLength(20)
                .HasColumnName("COMPANY");
            entity.Property(e => e.CreateAp)
                .HasMaxLength(50)
                .HasColumnName("CREATE_AP");
            entity.Property(e => e.CreateDate)
                .HasMaxLength(8)
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.CreatePrid)
                .HasMaxLength(50)
                .HasColumnName("CREATE_PRID");
            entity.Property(e => e.CreateTime)
                .HasMaxLength(20)
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.Creator)
                .HasMaxLength(10)
                .HasColumnName("CREATOR");
            entity.Property(e => e.Flag)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("FLAG");
            entity.Property(e => e.Mb002)
                .HasMaxLength(120)
                .HasDefaultValue("")
                .HasColumnName("MB002");
            entity.Property(e => e.Mb003)
                .HasMaxLength(120)
                .HasDefaultValue("")
                .HasColumnName("MB003");
            entity.Property(e => e.Mb004)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB004");
            entity.Property(e => e.Mb005)
                .HasMaxLength(40)
                .HasDefaultValue("")
                .HasColumnName("MB005");
            entity.Property(e => e.Mb006)
                .HasMaxLength(4)
                .HasDefaultValue("")
                .HasColumnName("MB006");
            entity.Property(e => e.Mb007)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MB007");
            entity.Property(e => e.Mb008)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasColumnName("MB008");
            entity.Property(e => e.Mb009)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MB009");
            entity.Property(e => e.Mb010)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasColumnName("MB010");
            entity.Property(e => e.Mb011)
                .HasMaxLength(6)
                .HasDefaultValue("")
                .HasColumnName("MB011");
            entity.Property(e => e.Mb012)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("MB012");
            entity.Property(e => e.Mb013)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB013");
            entity.Property(e => e.Mb014)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("MB014");
            entity.Property(e => e.Mb015)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("MB015");
            entity.Property(e => e.Mb016)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MB016");
            entity.Property(e => e.Mb017)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MB017");
            entity.Property(e => e.Mb018)
                .HasMaxLength(4)
                .HasDefaultValue("")
                .HasColumnName("MB018");
            entity.Property(e => e.Mb019)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB019");
            entity.Property(e => e.Mb020)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB020");
            entity.Property(e => e.Mb021)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB021");
            entity.Property(e => e.Mb022)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB022");
            entity.Property(e => e.Mb023)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB023");
            entity.Property(e => e.Mb024)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB024");
            entity.Property(e => e.Mb025)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB025");
            entity.Property(e => e.Mb026)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("MB026");
            entity.Property(e => e.Mb027)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB027");
            entity.Property(e => e.Mb028)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MB028");
            entity.Property(e => e.Mb029)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB029");
            entity.Property(e => e.Mb030)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MB030");
            entity.Property(e => e.Mb031)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MB031");
            entity.Property(e => e.Mb032)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MB032");
            entity.Property(e => e.Mb033)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB033");
            entity.Property(e => e.Mb034)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(8, 5)")
                .HasColumnName("MB034");
            entity.Property(e => e.Mb035)
                .HasMaxLength(60)
                .HasDefaultValue("")
                .HasColumnName("MB035");
            entity.Property(e => e.Mb036)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MB036");
            entity.Property(e => e.Mb037)
                .HasMaxLength(60)
                .HasDefaultValue("")
                .HasColumnName("MB037");
            entity.Property(e => e.Mb038)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MB038");
            entity.Property(e => e.Mb039)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB039");
            entity.Property(e => e.Mb040)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MB040");
            entity.Property(e => e.Mb041)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("MB041");
            entity.Property(e => e.Mb042)
                .HasMaxLength(4)
                .HasDefaultValue("")
                .HasColumnName("MB042");
            entity.Property(e => e.Mb043)
                .HasMaxLength(11)
                .HasDefaultValue("")
                .HasColumnName("MB043");
            entity.Property(e => e.Mb044)
                .HasMaxLength(4)
                .HasDefaultValue("")
                .HasColumnName("MB044");
            entity.Property(e => e.Mb045)
                .HasMaxLength(11)
                .HasDefaultValue("")
                .HasColumnName("MB045");
            entity.Property(e => e.Mb046)
                .HasMaxLength(4)
                .HasDefaultValue("")
                .HasColumnName("MB046");
            entity.Property(e => e.Mb047)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MB047");
            entity.Property(e => e.Mb048)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MB048");
            entity.Property(e => e.Mb049)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB049");
            entity.Property(e => e.Mb050)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB050");
            entity.Property(e => e.Mb051)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB051");
            entity.Property(e => e.Mb052)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB052");
            entity.Property(e => e.Mb053)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(20, 9)")
                .HasColumnName("MB053");
            entity.Property(e => e.Mb054)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB054");
            entity.Property(e => e.Mb055)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MB055");
            entity.Property(e => e.Mb056)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB056");
            entity.Property(e => e.Mb057)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasColumnName("MB057");
            entity.Property(e => e.Mb058)
                .HasMaxLength(60)
                .HasDefaultValue("")
                .HasColumnName("MB058");
            entity.Property(e => e.Mb059)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB059");
            entity.Property(e => e.Mb060)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB060");
            entity.Property(e => e.Mb061)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB061");
            entity.Property(e => e.Mb062)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("MB062");
            entity.Property(e => e.Mb063)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("MB063");
            entity.Property(e => e.Mb064)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB064");
            entity.Property(e => e.Mb065)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB065");
            entity.Property(e => e.Mb066)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB066");
            entity.Property(e => e.Mb067)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MB067");
            entity.Property(e => e.Mb068)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("MB068");
            entity.Property(e => e.Mb069)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("MB069");
            entity.Property(e => e.Mb070)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MB070");
            entity.Property(e => e.Mb071)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MB071");
            entity.Property(e => e.Mb072)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB072");
            entity.Property(e => e.Mb073)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB073");
            entity.Property(e => e.Mb074)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB074");
            entity.Property(e => e.Mb075)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB075");
            entity.Property(e => e.Mb076)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB076");
            entity.Property(e => e.Mb077)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB077");
            entity.Property(e => e.Mb078)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB078");
            entity.Property(e => e.Mb079)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MB079");
            entity.Property(e => e.ModiAp)
                .HasMaxLength(50)
                .HasColumnName("MODI_AP");
            entity.Property(e => e.ModiDate)
                .HasMaxLength(8)
                .HasColumnName("MODI_DATE");
            entity.Property(e => e.ModiPrid)
                .HasMaxLength(50)
                .HasColumnName("MODI_PRID");
            entity.Property(e => e.ModiTime)
                .HasMaxLength(20)
                .HasColumnName("MODI_TIME");
            entity.Property(e => e.Modifier)
                .HasMaxLength(10)
                .HasColumnName("MODIFIER");
            entity.Property(e => e.Udf01)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF01");
            entity.Property(e => e.Udf02)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF02");
            entity.Property(e => e.Udf03)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF03");
            entity.Property(e => e.Udf04)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF04");
            entity.Property(e => e.Udf05)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF05");
            entity.Property(e => e.Udf06)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF06");
            entity.Property(e => e.Udf07)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF07");
            entity.Property(e => e.Udf08)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF08");
            entity.Property(e => e.Udf09)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF09");
            entity.Property(e => e.Udf10)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF10");
            entity.Property(e => e.UsrGroup)
                .HasMaxLength(10)
                .HasColumnName("USR_GROUP");
        });

        modelBuilder.Entity<Astmc>(entity =>
        {
            entity.HasKey(e => new { e.Mc001, e.Mc002, e.Mc003 });

            entity.ToTable("ASTMC");

            entity.Property(e => e.Mc001)
                .HasMaxLength(40)
                .IsFixedLength()
                .HasColumnName("MC001");
            entity.Property(e => e.Mc002)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MC002");
            entity.Property(e => e.Mc003)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MC003");
            entity.Property(e => e.Company)
                .HasMaxLength(20)
                .HasColumnName("COMPANY");
            entity.Property(e => e.CreateAp)
                .HasMaxLength(50)
                .HasColumnName("CREATE_AP");
            entity.Property(e => e.CreateDate)
                .HasMaxLength(8)
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.CreatePrid)
                .HasMaxLength(50)
                .HasColumnName("CREATE_PRID");
            entity.Property(e => e.CreateTime)
                .HasMaxLength(20)
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.Creator)
                .HasMaxLength(10)
                .HasColumnName("CREATOR");
            entity.Property(e => e.Flag)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("FLAG");
            entity.Property(e => e.Mc004)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(5, 0)")
                .HasColumnName("MC004");
            entity.Property(e => e.Mc005)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MC005");
            entity.Property(e => e.Mc006)
                .HasMaxLength(60)
                .HasDefaultValue("")
                .HasColumnName("MC006");
            entity.Property(e => e.Mc007)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MC007");
            entity.Property(e => e.Mc008)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MC008");
            entity.Property(e => e.Mc009)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MC009");
            entity.Property(e => e.Mc010)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasColumnName("MC010");
            entity.Property(e => e.Mc011)
                .HasMaxLength(60)
                .HasDefaultValue("")
                .HasColumnName("MC011");
            entity.Property(e => e.ModiAp)
                .HasMaxLength(50)
                .HasColumnName("MODI_AP");
            entity.Property(e => e.ModiDate)
                .HasMaxLength(8)
                .HasColumnName("MODI_DATE");
            entity.Property(e => e.ModiPrid)
                .HasMaxLength(50)
                .HasColumnName("MODI_PRID");
            entity.Property(e => e.ModiTime)
                .HasMaxLength(20)
                .HasColumnName("MODI_TIME");
            entity.Property(e => e.Modifier)
                .HasMaxLength(10)
                .HasColumnName("MODIFIER");
            entity.Property(e => e.Udf01)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF01");
            entity.Property(e => e.Udf02)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF02");
            entity.Property(e => e.Udf03)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF03");
            entity.Property(e => e.Udf04)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF04");
            entity.Property(e => e.Udf05)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF05");
            entity.Property(e => e.Udf06)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF06");
            entity.Property(e => e.Udf07)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF07");
            entity.Property(e => e.Udf08)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF08");
            entity.Property(e => e.Udf09)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF09");
            entity.Property(e => e.Udf10)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF10");
            entity.Property(e => e.UsrGroup)
                .HasMaxLength(10)
                .HasColumnName("USR_GROUP");
        });

        modelBuilder.Entity<Cmsme>(entity =>
        {
            entity.HasKey(e => e.Me001);

            entity.ToTable("CMSME");

            entity.Property(e => e.Me001)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("ME001");
            entity.Property(e => e.Company)
                .HasMaxLength(20)
                .HasColumnName("COMPANY");
            entity.Property(e => e.CreateAp)
                .HasMaxLength(50)
                .HasColumnName("CREATE_AP");
            entity.Property(e => e.CreateDate)
                .HasMaxLength(8)
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.CreatePrid)
                .HasMaxLength(50)
                .HasColumnName("CREATE_PRID");
            entity.Property(e => e.CreateTime)
                .HasMaxLength(20)
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.Creator)
                .HasMaxLength(10)
                .HasColumnName("CREATOR");
            entity.Property(e => e.Flag)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("FLAG");
            entity.Property(e => e.Me002)
                .HasMaxLength(40)
                .HasDefaultValue("")
                .HasColumnName("ME002");
            entity.Property(e => e.Me003)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("ME003");
            entity.Property(e => e.Me004)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("ME004");
            entity.Property(e => e.Me005)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("ME005");
            entity.Property(e => e.Me006)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("ME006");
            entity.Property(e => e.Me007)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("ME007");
            entity.Property(e => e.Me008)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("ME008");
            entity.Property(e => e.Me009)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasColumnName("ME009");
            entity.Property(e => e.Me010)
                .HasMaxLength(60)
                .HasDefaultValue("")
                .HasColumnName("ME010");
            entity.Property(e => e.ModiAp)
                .HasMaxLength(50)
                .HasColumnName("MODI_AP");
            entity.Property(e => e.ModiDate)
                .HasMaxLength(8)
                .HasColumnName("MODI_DATE");
            entity.Property(e => e.ModiPrid)
                .HasMaxLength(50)
                .HasColumnName("MODI_PRID");
            entity.Property(e => e.ModiTime)
                .HasMaxLength(20)
                .HasColumnName("MODI_TIME");
            entity.Property(e => e.Modifier)
                .HasMaxLength(10)
                .HasColumnName("MODIFIER");
            entity.Property(e => e.Udf01)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF01");
            entity.Property(e => e.Udf02)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF02");
            entity.Property(e => e.Udf03)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF03");
            entity.Property(e => e.Udf04)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF04");
            entity.Property(e => e.Udf05)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF05");
            entity.Property(e => e.Udf06)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF06");
            entity.Property(e => e.Udf07)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF07");
            entity.Property(e => e.Udf08)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF08");
            entity.Property(e => e.Udf09)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF09");
            entity.Property(e => e.Udf10)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF10");
            entity.Property(e => e.UsrGroup)
                .HasMaxLength(10)
                .HasColumnName("USR_GROUP");
        });

        modelBuilder.Entity<Cmsmv>(entity =>
        {
            entity.HasKey(e => e.Mv001);

            entity.ToTable("CMSMV");

            entity.Property(e => e.Mv001)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("MV001");
            entity.Property(e => e.Company)
                .HasMaxLength(20)
                .HasColumnName("COMPANY");
            entity.Property(e => e.CreateAp)
                .HasMaxLength(50)
                .HasColumnName("CREATE_AP");
            entity.Property(e => e.CreateDate)
                .HasMaxLength(8)
                .HasColumnName("CREATE_DATE");
            entity.Property(e => e.CreatePrid)
                .HasMaxLength(50)
                .HasColumnName("CREATE_PRID");
            entity.Property(e => e.CreateTime)
                .HasMaxLength(20)
                .HasColumnName("CREATE_TIME");
            entity.Property(e => e.Creator)
                .HasMaxLength(10)
                .HasColumnName("CREATOR");
            entity.Property(e => e.Flag)
                .HasColumnType("numeric(3, 0)")
                .HasColumnName("FLAG");
            entity.Property(e => e.ModiAp)
                .HasMaxLength(50)
                .HasColumnName("MODI_AP");
            entity.Property(e => e.ModiDate)
                .HasMaxLength(8)
                .HasColumnName("MODI_DATE");
            entity.Property(e => e.ModiPrid)
                .HasMaxLength(50)
                .HasColumnName("MODI_PRID");
            entity.Property(e => e.ModiTime)
                .HasMaxLength(20)
                .HasColumnName("MODI_TIME");
            entity.Property(e => e.Modifier)
                .HasMaxLength(10)
                .HasColumnName("MODIFIER");
            entity.Property(e => e.Mv002)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasColumnName("MV002");
            entity.Property(e => e.Mv003)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV003");
            entity.Property(e => e.Mv004)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV004");
            entity.Property(e => e.Mv005)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV005");
            entity.Property(e => e.Mv006)
                .HasMaxLength(6)
                .HasDefaultValue("")
                .HasColumnName("MV006");
            entity.Property(e => e.Mv007)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV007");
            entity.Property(e => e.Mv008)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV008");
            entity.Property(e => e.Mv009)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MV009");
            entity.Property(e => e.Mv010)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV010");
            entity.Property(e => e.Mv011)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV011");
            entity.Property(e => e.Mv012)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV012");
            entity.Property(e => e.Mv013)
                .HasMaxLength(80)
                .HasDefaultValue("")
                .HasColumnName("MV013");
            entity.Property(e => e.Mv014)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV014");
            entity.Property(e => e.Mv015)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MV015");
            entity.Property(e => e.Mv016)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MV016");
            entity.Property(e => e.Mv017)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MV017");
            entity.Property(e => e.Mv018)
                .HasMaxLength(6)
                .HasDefaultValue("")
                .HasColumnName("MV018");
            entity.Property(e => e.Mv019)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MV019");
            entity.Property(e => e.Mv020)
                .HasMaxLength(60)
                .HasDefaultValue("")
                .HasColumnName("MV020");
            entity.Property(e => e.Mv021)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV021");
            entity.Property(e => e.Mv022)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV022");
            entity.Property(e => e.Mv023)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV023");
            entity.Property(e => e.Mv024)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV024");
            entity.Property(e => e.Mv025)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV025");
            entity.Property(e => e.Mv026)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV026");
            entity.Property(e => e.Mv027)
                .HasMaxLength(3)
                .HasDefaultValue("")
                .HasColumnName("MV027");
            entity.Property(e => e.Mv028)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV028");
            entity.Property(e => e.Mv029)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV029");
            entity.Property(e => e.Mv030)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV030");
            entity.Property(e => e.Mv031)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(4, 2)")
                .HasColumnName("MV031");
            entity.Property(e => e.Mv032)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV032");
            entity.Property(e => e.Mv033)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV033");
            entity.Property(e => e.Mv034)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV034");
            entity.Property(e => e.Mv035)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MV035");
            entity.Property(e => e.Mv036)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasColumnName("MV036");
            entity.Property(e => e.Mv037)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(2, 0)")
                .HasColumnName("MV037");
            entity.Property(e => e.Mv038)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV038");
            entity.Property(e => e.Mv039)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV039");
            entity.Property(e => e.Mv040)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(8, 5)")
                .HasColumnName("MV040");
            entity.Property(e => e.Mv041)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV041");
            entity.Property(e => e.Mv042)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV042");
            entity.Property(e => e.Mv043)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV043");
            entity.Property(e => e.Mv044)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV044");
            entity.Property(e => e.Mv045)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV045");
            entity.Property(e => e.Mv046)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MV046");
            entity.Property(e => e.Mv047)
                .HasMaxLength(80)
                .HasDefaultValue("")
                .HasColumnName("MV047");
            entity.Property(e => e.Mv048)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV048");
            entity.Property(e => e.Mv049)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV049");
            entity.Property(e => e.Mv050)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV050");
            entity.Property(e => e.Mv051)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MV051");
            entity.Property(e => e.Mv052)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV052");
            entity.Property(e => e.Mv053)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV053");
            entity.Property(e => e.Mv054)
                .HasMaxLength(2)
                .HasDefaultValue("")
                .HasColumnName("MV054");
            entity.Property(e => e.Mv055)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV055");
            entity.Property(e => e.Mv056)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV056");
            entity.Property(e => e.Mv057)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV057");
            entity.Property(e => e.Mv058)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MV058");
            entity.Property(e => e.Mv059)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV059");
            entity.Property(e => e.Mv060)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV060");
            entity.Property(e => e.Mv061)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV061");
            entity.Property(e => e.Mv062)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MV062");
            entity.Property(e => e.Mv063)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MV063");
            entity.Property(e => e.Mv064)
                .HasMaxLength(6)
                .HasDefaultValue("")
                .HasColumnName("MV064");
            entity.Property(e => e.Mv065)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV065");
            entity.Property(e => e.Mv066)
                .HasMaxLength(6)
                .HasDefaultValue("")
                .HasColumnName("MV066");
            entity.Property(e => e.Mv067)
                .HasMaxLength(6)
                .HasDefaultValue("")
                .HasColumnName("MV067");
            entity.Property(e => e.Mv068)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV068");
            entity.Property(e => e.Mv069)
                .HasMaxLength(80)
                .HasDefaultValue("")
                .HasColumnName("MV069");
            entity.Property(e => e.Mv070)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV070");
            entity.Property(e => e.Mv071)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV071");
            entity.Property(e => e.Mv072)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV072");
            entity.Property(e => e.Mv073)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV073");
            entity.Property(e => e.Mv074)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV074");
            entity.Property(e => e.Mv075)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV075");
            entity.Property(e => e.Mv076)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(8, 5)")
                .HasColumnName("MV076");
            entity.Property(e => e.Mv077)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV077");
            entity.Property(e => e.Mv078)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(4, 2)")
                .HasColumnName("MV078");
            entity.Property(e => e.Mv079)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(8, 5)")
                .HasColumnName("MV079");
            entity.Property(e => e.Mv080)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(2, 0)")
                .HasColumnName("MV080");
            entity.Property(e => e.Mv081)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV081");
            entity.Property(e => e.Mv082)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MV082");
            entity.Property(e => e.Mv083)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV083");
            entity.Property(e => e.Mv084)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV084");
            entity.Property(e => e.Mv085)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasColumnName("MV085");
            entity.Property(e => e.Mv086)
                .HasMaxLength(60)
                .HasDefaultValue("")
                .HasColumnName("MV086");
            entity.Property(e => e.Mv087)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV087");
            entity.Property(e => e.Mv088)
                .HasMaxLength(12)
                .HasDefaultValue("")
                .HasColumnName("MV088");
            entity.Property(e => e.Mv089)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MV089");
            entity.Property(e => e.Mv090)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV090");
            entity.Property(e => e.Mv091)
                .HasMaxLength(12)
                .HasDefaultValue("")
                .HasColumnName("MV091");
            entity.Property(e => e.Mv092)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV092");
            entity.Property(e => e.Mv093)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV093");
            entity.Property(e => e.Mv094)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV094");
            entity.Property(e => e.Mv095)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV095");
            entity.Property(e => e.Mv096)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV096");
            entity.Property(e => e.Mv097)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV097");
            entity.Property(e => e.Mv098)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV098");
            entity.Property(e => e.Mv099)
                .HasMaxLength(40)
                .HasDefaultValue("")
                .HasColumnName("MV099");
            entity.Property(e => e.Mv100)
                .HasMaxLength(4)
                .HasDefaultValue("")
                .HasColumnName("MV100");
            entity.Property(e => e.Mv101)
                .HasMaxLength(20)
                .HasDefaultValue("")
                .HasColumnName("MV101");
            entity.Property(e => e.Mv102)
                .HasMaxLength(4)
                .HasDefaultValue("")
                .HasColumnName("MV102");
            entity.Property(e => e.Mv103)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV103");
            entity.Property(e => e.Mv104)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(15, 6)")
                .HasColumnName("MV104");
            entity.Property(e => e.Mv105)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV105");
            entity.Property(e => e.Mv106)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV106");
            entity.Property(e => e.Mv107)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV107");
            entity.Property(e => e.Mv108)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV108");
            entity.Property(e => e.Mv109)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV109");
            entity.Property(e => e.Mv110)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV110");
            entity.Property(e => e.Mv111)
                .HasMaxLength(10)
                .HasDefaultValue("")
                .HasColumnName("MV111");
            entity.Property(e => e.Mv112)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV112");
            entity.Property(e => e.Mv113)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV113");
            entity.Property(e => e.Mv114)
                .HasMaxLength(40)
                .HasDefaultValue("")
                .HasColumnName("MV114");
            entity.Property(e => e.Mv115)
                .HasMaxLength(40)
                .HasDefaultValue("")
                .HasColumnName("MV115");
            entity.Property(e => e.Mv116)
                .HasMaxLength(40)
                .HasDefaultValue("")
                .HasColumnName("MV116");
            entity.Property(e => e.Mv117)
                .HasMaxLength(2)
                .HasDefaultValue("")
                .HasColumnName("MV117");
            entity.Property(e => e.Mv118)
                .HasMaxLength(6)
                .HasDefaultValue("")
                .HasColumnName("MV118");
            entity.Property(e => e.Mv119)
                .HasMaxLength(8)
                .HasDefaultValue("")
                .HasColumnName("MV119");
            entity.Property(e => e.Mv120)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(4, 2)")
                .HasColumnName("MV120");
            entity.Property(e => e.Mv121)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV121");
            entity.Property(e => e.Mv122)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MV122");
            entity.Property(e => e.Mv123)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MV123");
            entity.Property(e => e.Mv124)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MV124");
            entity.Property(e => e.Mv125)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("MV125");
            entity.Property(e => e.Mv126)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MV126");
            entity.Property(e => e.Mv127)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MV127");
            entity.Property(e => e.Mv128)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MV128");
            entity.Property(e => e.Mv129)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("MV129");
            entity.Property(e => e.Mv130)
                .HasMaxLength(1)
                .HasDefaultValue("")
                .HasColumnName("MV130");
            entity.Property(e => e.Udf01)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF01");
            entity.Property(e => e.Udf02)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF02");
            entity.Property(e => e.Udf03)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF03");
            entity.Property(e => e.Udf04)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF04");
            entity.Property(e => e.Udf05)
                .HasMaxLength(255)
                .HasDefaultValue("")
                .HasColumnName("UDF05");
            entity.Property(e => e.Udf06)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF06");
            entity.Property(e => e.Udf07)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF07");
            entity.Property(e => e.Udf08)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF08");
            entity.Property(e => e.Udf09)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF09");
            entity.Property(e => e.Udf10)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(21, 6)")
                .HasColumnName("UDF10");
            entity.Property(e => e.UsrGroup)
                .HasMaxLength(10)
                .HasColumnName("USR_GROUP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
