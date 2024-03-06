using System;
using System.Collections.Generic;
using Dreamcore_Horror_Game_API_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server;

public partial class DreamcoreHorrorGameContext : DbContext
{
    public DreamcoreHorrorGameContext()
    {
    }

    public DreamcoreHorrorGameContext(DbContextOptions<DreamcoreHorrorGameContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ability> Abilities { get; set; }

    public virtual DbSet<AcquiredAbility> AcquiredAbilities { get; set; }

    public virtual DbSet<Artifact> Artifacts { get; set; }

    public virtual DbSet<CollectedArtifact> CollectedArtifacts { get; set; }

    public virtual DbSet<Creature> Creatures { get; set; }

    public virtual DbSet<ExperienceLevel> ExperienceLevels { get; set; }

    public virtual DbSet<GameMode> GameModes { get; set; }

    public virtual DbSet<GameSession> GameSessions { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerSession> PlayerSessions { get; set; }

    public virtual DbSet<RarityLevel> RarityLevels { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=dreamcore_horror_game;Username=postgres;Password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ability>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("abilities_pkey");

            entity.ToTable("abilities");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AssetName)
                .HasMaxLength(255)
                .HasColumnName("asset_name");
        });

        modelBuilder.Entity<AcquiredAbility>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("acquired_abilities_pkey");

            entity.ToTable("acquired_abilities");

            entity.HasIndex(e => e.AbilityId, "fki_acquired_abilities_fkey_ability_id");

            entity.HasIndex(e => e.PlayerId, "fki_acquired_abilities_fkey_player_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AbilityId).HasColumnName("ability_id");
            entity.Property(e => e.AcquirementTimestamp).HasColumnName("acquirement_timestamp");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");

            entity.HasOne(d => d.Ability).WithMany(p => p.AcquiredAbilities)
                .HasForeignKey(d => d.AbilityId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("acquired_abilities_fkey_ability_id");

            entity.HasOne(d => d.Player).WithMany(p => p.AcquiredAbilities)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("acquired_abilities_fkey_player_id");
        });

        modelBuilder.Entity<Artifact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("artifacts_pkey");

            entity.ToTable("artifacts");

            entity.HasIndex(e => e.RarityLevelId, "fki_artifacts_fkey_rarity_level_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AssetName)
                .HasMaxLength(255)
                .HasColumnName("asset_name");
            entity.Property(e => e.RarityLevelId).HasColumnName("rarity_level_id");

            entity.HasOne(d => d.RarityLevel).WithMany(p => p.Artifacts)
                .HasForeignKey(d => d.RarityLevelId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("artifacts_fkey_rarity_level_id");
        });

        modelBuilder.Entity<CollectedArtifact>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("collected_artifacts_pkey");

            entity.ToTable("collected_artifacts");

            entity.HasIndex(e => e.ArtifactId, "fki_collected_artifacts_fkey_artifact_id");

            entity.HasIndex(e => e.PlayerId, "fki_collected_artifacts_fkey_player_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ArtifactId).HasColumnName("artifact_id");
            entity.Property(e => e.CollectionTimestamp).HasColumnName("collection_timestamp");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");

            entity.HasOne(d => d.Artifact).WithMany(p => p.CollectedArtifacts)
                .HasForeignKey(d => d.ArtifactId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("collected_artifacts_fkey_artifact_id");

            entity.HasOne(d => d.Player).WithMany(p => p.CollectedArtifacts)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("collected_artifacts_fkey_player_id");
        });

        modelBuilder.Entity<Creature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("creatures_pkey");

            entity.ToTable("creatures");

            entity.HasIndex(e => e.RequiredExperienceLevelId, "fki_creatures_fkey_required_experience_level_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AssetName)
                .HasMaxLength(255)
                .HasColumnName("asset_name");
            entity.Property(e => e.Health).HasColumnName("health");
            entity.Property(e => e.MovementSpeed).HasColumnName("movement_speed");
            entity.Property(e => e.RequiredExperienceLevelId).HasColumnName("required_experience_level_id");

            entity.HasOne(d => d.RequiredExperienceLevel).WithMany(p => p.Creatures)
                .HasForeignKey(d => d.RequiredExperienceLevelId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("creatures_fkey_required_experience_level_id");
        });

        modelBuilder.Entity<ExperienceLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("experience_levels_pkey");

            entity.ToTable("experience_levels");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.RequiredExperiencePoints).HasColumnName("required_experience_points");
        });

        modelBuilder.Entity<GameMode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_modes_pkey");

            entity.ToTable("game_modes");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AssetName)
                .HasMaxLength(255)
                .HasColumnName("asset_name");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("true")
                .HasColumnName("is_active");
            entity.Property(e => e.MaxPlayers).HasColumnName("max_players");
            entity.Property(e => e.TimeLimit).HasColumnName("time_limit");
        });

        modelBuilder.Entity<GameSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("game_sessions_pkey");

            entity.ToTable("game_sessions");

            entity.HasIndex(e => e.GameModeId, "fki_game_sessions_fkey_game_mode_id");

            entity.HasIndex(e => e.ServerId, "fki_game_sessions_fkey_server_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EndTimestamp).HasColumnName("end_timestamp");
            entity.Property(e => e.GameModeId).HasColumnName("game_mode_id");
            entity.Property(e => e.ServerId).HasColumnName("server_id");
            entity.Property(e => e.StartTimestamp).HasColumnName("start_timestamp");

            entity.HasOne(d => d.GameMode).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.GameModeId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("game_sessions_fkey_game_mode_id");

            entity.HasOne(d => d.Server).WithMany(p => p.GameSessions)
                .HasForeignKey(d => d.ServerId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("game_sessions_fkey_server_id");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("players_pkey");

            entity.ToTable("players");

            entity.HasIndex(e => e.ExperienceLevelId, "fki_players_fkey_experience_level_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AbilityPoints).HasColumnName("ability_points");
            entity.Property(e => e.CollectOptionalData).HasColumnName("collect_optional_data");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.ExperienceLevelId).HasColumnName("experience_level_id");
            entity.Property(e => e.ExperiencePoints).HasColumnName("experience_points");
            entity.Property(e => e.IsOnline).HasColumnName("is_online");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RegistrationTimestamp).HasColumnName("registration_timestamp");
            entity.Property(e => e.SpiritEnergyPoints).HasColumnName("spirit_energy_points");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");

            entity.HasOne(d => d.ExperienceLevel).WithMany(p => p.Players)
                .HasForeignKey(d => d.ExperienceLevelId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("players_fkey_experience_level_id");
        });

        modelBuilder.Entity<PlayerSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("player_sessions_pkey");

            entity.ToTable("player_sessions");

            entity.HasIndex(e => e.GameSessionId, "fki_player_sessions_fkey_game_session_id");

            entity.HasIndex(e => e.PlayerId, "fki_player_sessions_fkey_player_id");

            entity.HasIndex(e => e.UsedCreatureId, "fki_player_sessions_fkey_used_creature_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.EndTimestamp).HasColumnName("end_timestamp");
            entity.Property(e => e.GameSessionId).HasColumnName("game_session_id");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed");
            entity.Property(e => e.IsWon).HasColumnName("is_won");
            entity.Property(e => e.PlayedAsCreature).HasColumnName("played_as_creature");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.StartTimestamp).HasColumnName("start_timestamp");
            entity.Property(e => e.TimeAlive).HasColumnName("time_alive");
            entity.Property(e => e.UsedCreatureId).HasColumnName("used_creature_id");

            entity.HasOne(d => d.GameSession).WithMany(p => p.PlayerSessions)
                .HasForeignKey(d => d.GameSessionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("player_sessions_fkey_game_session_id");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerSessions)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("player_sessions_fkey_player_id");

            entity.HasOne(d => d.UsedCreature).WithMany(p => p.PlayerSessions)
                .HasForeignKey(d => d.UsedCreatureId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("player_sessions_fkey_used_creature_id");
        });

        modelBuilder.Entity<RarityLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rarity_levels_pkey");

            entity.ToTable("rarity_levels");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AssetName)
                .HasMaxLength(255)
                .HasColumnName("asset_name");
            entity.Property(e => e.Probability).HasColumnName("probability");
        });

        modelBuilder.Entity<Server>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("servers_pkey");

            entity.ToTable("servers");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address");
            entity.Property(e => e.IsOnline).HasColumnName("is_online");
            entity.Property(e => e.PlayerCapacity).HasColumnName("player_capacity");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
