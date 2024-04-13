using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DreamcoreHorrorGameApiServer.Models.Database;

namespace DreamcoreHorrorGameApiServer;

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

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<DeveloperAccessLevel> DeveloperAccessLevels { get; set; }

    public virtual DbSet<GameMode> GameModes { get; set; }

    public virtual DbSet<GameSession> GameSessions { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerSession> PlayerSessions { get; set; }

    public virtual DbSet<RarityLevel> RarityLevels { get; set; }

    public virtual DbSet<Server> Servers { get; set; }

    public virtual DbSet<XpLevel> XpLevels { get; set; }

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

            entity.HasIndex(e => e.RequiredXpLevelId, "fki_creatures_fkey_required_experience_level_id");

            entity.HasIndex(e => e.RequiredXpLevelId, "fki_creatures_pkey_required_xp_level_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AssetName)
                .HasMaxLength(255)
                .HasColumnName("asset_name");
            entity.Property(e => e.Health).HasColumnName("health");
            entity.Property(e => e.MovementSpeed).HasColumnName("movement_speed");
            entity.Property(e => e.RequiredXpLevelId).HasColumnName("required_xp_level_id");

            entity.HasOne(d => d.RequiredXpLevel).WithMany(p => p.Creatures)
                .HasForeignKey(d => d.RequiredXpLevelId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("creatures_pkey_required_xp_level_id");
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("developers_pkey");

            entity.ToTable("developers");

            entity.HasIndex(e => e.DeveloperAccessLevelId, "fki_developers_fkey_developer_access_level_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DeveloperAccessLevelId).HasColumnName("developer_access_level_id");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RefreshToken)
                .HasColumnType("character varying")
                .HasColumnName("refresh_token");

            entity.HasOne(d => d.DeveloperAccessLevel).WithMany(p => p.Developers)
                .HasForeignKey(d => d.DeveloperAccessLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("developers_fkey_developer_access_level_id");
        });

        modelBuilder.Entity<DeveloperAccessLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("developer_access_levels_pkey");

            entity.ToTable("developer_access_levels");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
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

            entity.HasIndex(e => e.XpLevelId, "fki_players_fkey_xp_level_id");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AbilityPoints).HasColumnName("ability_points");
            entity.Property(e => e.CollectOptionalData).HasColumnName("collect_optional_data");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsOnline).HasColumnName("is_online");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.RefreshToken)
                .HasColumnType("character varying")
                .HasColumnName("refresh_token");
            entity.Property(e => e.RegistrationTimestamp).HasColumnName("registration_timestamp");
            entity.Property(e => e.SpiritEnergyPoints).HasColumnName("spirit_energy_points");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");
            entity.Property(e => e.Xp).HasColumnName("xp");
            entity.Property(e => e.XpLevelId).HasColumnName("xp_level_id");

            entity.HasOne(d => d.XpLevel).WithMany(p => p.Players)
                .HasForeignKey(d => d.XpLevelId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("players_fkey_xp_level_id");
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
            entity.Property(e => e.AllyReviveCount).HasColumnName("ally_revive_count");
            entity.Property(e => e.EndTimestamp).HasColumnName("end_timestamp");
            entity.Property(e => e.GameSessionId).HasColumnName("game_session_id");
            entity.Property(e => e.IsCompleted).HasColumnName("is_completed");
            entity.Property(e => e.IsWon).HasColumnName("is_won");
            entity.Property(e => e.PlayedAsCreature).HasColumnName("played_as_creature");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.SelfReviveCount).HasColumnName("self_revive_count");
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
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.PlayerCapacity).HasColumnName("player_capacity");
            entity.Property(e => e.RefreshToken)
                .HasColumnType("character varying")
                .HasColumnName("refresh_token");
        });

        modelBuilder.Entity<XpLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("experience_levels_pkey");

            entity.ToTable("xp_levels");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Number).HasColumnName("number");
            entity.Property(e => e.RequiredXp).HasColumnName("required_xp");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
