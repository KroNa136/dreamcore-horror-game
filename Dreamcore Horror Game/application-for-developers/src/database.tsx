import { toReadableUtcDateTime } from "./value-format-helper";

export function displayName(entity: DatabaseEntity | null | undefined): string | undefined {
  if (entity === null || entity === undefined) {
    return "NULL";
  }
  // checking for the existance of unique class fields to determine the type of entity
  if ((entity as Ability).acquiredAbilities !== undefined && (entity as Ability).assetName !== undefined) {
    return (entity as Ability).assetName;
  }
  else if ((entity as AcquiredAbility).acquirementTimestamp !== undefined) {
    return `${displayName((entity as AcquiredAbility).ability) ?? "NULL"} у игрока ${displayName((entity as AcquiredAbility).player) ?? "NULL"}`
  }
  else if ((entity as Artifact).rarityLevel !== undefined) {
    return (entity as Artifact).assetName;
  }
  else if ((entity as CollectedArtifact).collectionTimestamp !== undefined) {
    return `${displayName((entity as CollectedArtifact).artifact) ?? "NULL"} у игрока ${displayName((entity as CollectedArtifact).player) ?? "NULL"}`;
  }
  else if ((entity as Creature).requiredXpLevel !== undefined) {
    return (entity as Creature).assetName;
  }
  else if ((entity as Developer).login !== undefined) {
    return (entity as Developer).login;
  }
  else if ((entity as DeveloperAccessLevel).developers !== undefined) {
    return (entity as DeveloperAccessLevel).name;
  }
  else if ((entity as GameMode).timeLimit !== undefined) {
    return (entity as GameMode).assetName;
  }
  else if ((entity as GameSession).gameMode !== undefined) {
    return `На сервере ${displayName((entity as GameSession).server) ?? "NULL"} ${toReadableUtcDateTime((entity as GameSession).startTimestamp)}`;
  }
  else if ((entity as Player).abilityPoints !== undefined) {
    return (entity as Player).username;
  }
  else if ((entity as PlayerSession).playedAsCreature !== undefined) {
    if ((entity as PlayerSession).gameSession?.gameMode !== undefined && ((entity as PlayerSession).gameSession as GameSession).gameMode !== undefined) {
      return `У игрока ${displayName((entity as PlayerSession).player) ?? "NULL"} на сервере ${displayName(((entity as PlayerSession).gameSession as GameSession).server) ?? "NULL"} ${toReadableUtcDateTime((entity as PlayerSession).startTimestamp)}`;
    }
    else {
      return `У игрока ${displayName((entity as PlayerSession).player) ?? "NULL"} на сервере NULL ${toReadableUtcDateTime((entity as PlayerSession).startTimestamp)}`;
    }
  }
  else if ((entity as RarityLevel).probability !== undefined) {
    return (entity as RarityLevel).assetName;
  }
  else if ((entity as Server).playerCapacity !== undefined) {
    return (entity as Server).ipAddress;
  }
  else if ((entity as XpLevel).number !== undefined) {
    return (entity as XpLevel).number.toString();
  }
}

const defaultId = "00000000-0000-0000-0000-000000000000";
const defaultTimestamp = "1970-01-01 00:00:00";

export interface DatabaseEntity {
  id: string,
}

export class Ability implements DatabaseEntity {
  id: string = defaultId;
  assetName: string = "";
  acquiredAbilities: AcquiredAbility[] = [];
}

export class AcquiredAbility implements DatabaseEntity {
  id: string = defaultId;
  playerId: string = "";
  abilityId: string = "";
  acquirementTimestamp: string = defaultTimestamp;
  ability: Ability | undefined;
  player: Player | undefined;
}

export class Artifact implements DatabaseEntity {
  id: string = defaultId;
  assetName: string = "";
  rarityLevelId: string = "";
  collectedArtifacts: CollectedArtifact[] = [];
  rarityLevel: RarityLevel | undefined;
}

export class CollectedArtifact implements DatabaseEntity {
  id: string = defaultId;
  playerId: string = "";
  artifactId: string = "";
  collectionTimestamp: string = defaultTimestamp;
  artifact: Artifact | undefined;
  player: Player | undefined;
}

export class Creature implements DatabaseEntity {
  id: string = defaultId;
  assetName: string = "";
  requiredXpLevelId: string = "";
  health: number = 0;
  movementSpeed: number = 0;
  playerSessions: PlayerSession[] = [];
  requiredXpLevel: XpLevel | undefined;
}

export class Developer implements DatabaseEntity {
  id: string = defaultId;
  login: string = "";
  password: string = "";
  refreshToken: string | null = null;
  developerAccessLevelId: string = "";
  isOnline: boolean = false;
  developerAccessLevel: DeveloperAccessLevel | undefined;
}

export class DeveloperAccessLevel implements DatabaseEntity {
  id: string = defaultId;
  name: string = "";
  developers: Developer[] = [];
}

export class GameMode implements DatabaseEntity {
  id: string = defaultId;
  assetName: string = "";
  maxPlayers: number | null = null;
  timeLimit: string | null = null;
  isActive: boolean | null = null;
  gameSessions: GameSession[] = [];
}

export class GameSession implements DatabaseEntity {
  id: string = defaultId;
  serverId: string | null = null;
  gameModeId: string = "";
  startTimestamp: string = defaultTimestamp;
  endTimestamp: string | null = null;
  gameMode: GameMode | undefined;
  playerSessions: PlayerSession[] = [];
  server: Server | null = null;
}

export class Player implements DatabaseEntity {
  id: string = defaultId;
  username: string = "";
  email: string = "";
  password: string = "";
  refreshToken: string | null = null;
  registrationTimestamp: string = defaultTimestamp;
  collectOptionalData: boolean = false;
  isOnline: boolean = false;
  xpLevelId: string = "";
  xp: number = 0;
  abilityPoints: number = 0;
  spiritEnergyPoints: number = 0;
  acquiredAbilities: AcquiredAbility[] = [];
  collectedArtifacts: CollectedArtifact[] = [];
  playerSessions: PlayerSession[] = [];
  xpLevel: XpLevel | undefined;
}

export class PlayerSession implements DatabaseEntity {
  id: string = defaultId;
  gameSessionId: string = "";
  playerId: string = "";
  startTimestamp: string = defaultTimestamp;
  endTimestamp: string | null = null;
  isCompleted: boolean | null = null;
  isWon: boolean | null = null;
  timeAlive: string | null = null;
  playedAsCreature: boolean | null = null;
  usedCreatureId: string | null = null;
  selfReviveCount: number | null = null;
  allyReviveCount: number | null = null;
  gameSession: GameSession | undefined;
  player: Player | undefined;
  usedCreature: Creature | null = null;
}

export class RarityLevel implements DatabaseEntity {
  id: string = defaultId;
  assetName: string = "";
  probability: number = 0;
  artifacts: Artifact[] = [];
}

export class Server implements DatabaseEntity {
  id: string = defaultId;
  ipAddress: string = "";
  password: string = "";
  refreshToken: string | null = null;
  playerCapacity: number = 0;
  isOnline: boolean = false;
  gameSessions: GameSession[] = [];
}

export class XpLevel implements DatabaseEntity {
  id: string = defaultId;
  number: number = 0;
  requiredXp: number = 0;
  creatures: Creature[] = [];
  players: Player[] = [];
}

//============================== Backend GetAll Response Interfaces ==============================//

export interface Entities<TEntity> {
  items: TEntity[],
  pageCount: number
}

export interface Abilities extends Entities<Ability> { }
export interface AcquiredAbilities extends Entities<AcquiredAbility> { }
export interface Artifacts extends Entities<Artifact> { }
export interface CollectedArtifacts extends Entities<CollectedArtifact> { }
export interface Creatures extends Entities<Creature> { }
export interface Developers extends Entities<Developer> { }
export interface DeveloperAccessLevels extends Entities<DeveloperAccessLevel> { }
export interface GameModes extends Entities<GameMode> { }
export interface GameSessions extends Entities<GameSession> { }
export interface Players extends Entities<Player> { }
export interface PlayerSessions extends Entities<PlayerSession> { }
export interface RarityLevels extends Entities<RarityLevel> { }
export interface Servers extends Entities<Server> { }
export interface XpLevels extends Entities<XpLevel> { }
