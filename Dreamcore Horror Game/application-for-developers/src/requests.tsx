import axios from "axios";
import { getCurrentLogin, getCurrentAccessToken, getCurrentRefreshToken, signIn, setAccessToken, signOut } from "./auth-state";
import {
  Ability, AcquiredAbility, Artifact, CollectedArtifact, Creature, Developer, DeveloperAccessLevel, GameMode,
  GameSession, Player, PlayerSession, RarityLevel, Server, XpLevel, Abilities, AcquiredAbilities, Artifacts,
  CollectedArtifacts, Creatures, Developers, DeveloperAccessLevels, GameModes, GameSessions, Players,
  PlayerSessions, RarityLevels, Servers, XpLevels
} from "./database";

axios.defaults.baseURL = "https://localhost:8008/api/";
axios.defaults.headers.common = {
  "Content-Type": "application/json",
  "Application-For-Developers": "sWBlYN0yimNMCLXFofH7MBmdOIelMQEHlPJZ304Sdjsd47liU2M1Ilv2kjAQSKA2",
};

export interface LoginData {
  login: string,
  password: string,
}

//==================================================================================================

function handleError(error: any) {
  if (axios.isAxiosError(error)) {
    console.log(error.response?.data);
    alert(`Возникла ошибка HTTP${" ".concat(error.response?.data?.status?.toString()) ?? ""}. Подробности смотрите в консоли.`);
  }
  else if (error instanceof Error) {
    alert("Возникла ошибка при подключении к серверу: ".concat(error.message));
  }
}

//==================================================================================================

async function getAccessToken() {
  const currentAccessToken = getCurrentAccessToken();

  try {
    await axios.get("Developers/VerifyAccessToken", {
      headers: {
        Authorization: "Bearer ".concat(currentAccessToken)
      }
    });
    return currentAccessToken;
  }
  catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 401) {
      try {
        const response = await axios.get("Developers/GetAccessToken", {
          headers: {
            Authorization: "Bearer ".concat(getCurrentRefreshToken())
          },
          params: {
            login: getCurrentLogin()
          }
        });
        const accessToken = response.data as string;
        setAccessToken(accessToken);
        return accessToken;
      }
      catch (error) {
        if (axios.isAxiosError(error) && error.response?.status === 401) {
          alert("Закончился срок действия refresh-токена авторизации. Пожалуйста, выйдите из системы и войдите заново.");
          signOut();
        }
        else {
          handleError(error);
        }
        return "";
      }
    }
    else {
        handleError(error);
        return "";
    }
  }
}

export async function loginAsDeveloper(loginData: LoginData) : Promise<boolean> {
  try {
    const response = await axios.post("Developers/Login", loginData);
    const refreshToken = response.data as string;

    const developer = await getDeveloperByLogin(loginData.login);
    if (developer === undefined) {
      alert("Не удалось получить уровень доступа пользователя. Уровень доступа для текущего сеанса будет установлен на Низкий.");
    }
    const developerAccessLevel = developer !== undefined
      ? (developer.developerAccessLevel as DeveloperAccessLevel).name
      : "Low Access Developer";

    signIn(loginData.login, refreshToken, developerAccessLevel);
    await getAccessToken();
    return true;
  }
  catch (error) {
    handleError(error);
    return false;
  }
}

export async function logoutAsDeveloper(): Promise<boolean> {
  try {
    await axios.post("Developers/Logout", {}, {
      params: {
        login: getCurrentLogin()
      },
      headers: {
        Authorization: "Bearer ".concat(await getAccessToken())
      },
    });
    signOut();
    return true;
  }
  catch (error) {
    handleError(error);
    return false;
  }
}

async function getDeveloperByLogin(login: string): Promise<Developer | undefined> {
  try {
    const response = await axios.get(`Developers/GetByLogin`, {
      params: {
        login: login,
      },
    });
    return response.data as Developer;
  }
  catch (error) {
    handleError(error);
  }
}

//==================================================================================================

async function getCount(controllerUrl: string): Promise<number> {
  try {
    const response = await axios.get(`${controllerUrl}/GetCount`, {
      headers: {
        Authorization: "Bearer ".concat(await getAccessToken())
      },
    });
    return response.data as number;
  }
  catch (error) {
    handleError(error);
    return NaN;
  }
}

export const getAbilityCount = async () =>
  await getCount("Abilities");

export const getAcquiredAbilityCount = async () =>
  await getCount("AcquiredAbilities");

export const getArtifactCount = async () =>
  await getCount("Artifacts");

export const getCollectedArtifactCount = async () =>
  await getCount("CollectedArtifacts");

export const getCreatureCount = async () =>
  await getCount("Creatures");

export const getDeveloperCount = async () =>
  await getCount("Developers");

export const getDeveloperAccessLevelCount = async () =>
  await getCount("DeveloperAccessLevels");

export const getGameModeCount = async () =>
  await getCount("GameModes");

export const getGameSessionCount = async () =>
  await getCount("GameSessions");

export const getPlayerCount = async () =>
  await getCount("Players");

export const getPlayerSessionCount = async () =>
  await getCount("PlayerSessions");

export const getRarityLevelCount = async () =>
  await getCount("RarityLevels");

export const getServerCount = async () =>
  await getCount("Servers");

export const getXpLevelCount = async () =>
  await getCount("XpLevels");

//==================================================================================================

async function getAllWithRelations<TEntities>(controllerUrl: string, page: number | undefined, showBy: number | undefined, empty: TEntities): Promise<TEntities> {
  try {
    const response = await axios.get(`${controllerUrl}/GetAllWithRelations`, {
      headers: {
        Authorization: "Bearer ".concat(await getAccessToken())
      },
      params: {
        page: page,
        showBy: showBy
      }
    });
    return response.data as TEntities;
  }
  catch (error) {
    handleError(error);
    return empty;
  }
}

export const getAbilities = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<Abilities>("Abilities", page, showBy, { items: [], pageCount: 0 });

export const getAcquiredAbilities = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<AcquiredAbilities>("AcquiredAbilities", page, showBy, { items: [], pageCount: 0 });

export const getArtifacts = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<Artifacts>("Artifacts", page, showBy, { items: [], pageCount: 0 });

export const getCollectedArtifacts = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<CollectedArtifacts>("CollectedArtifacts", page, showBy, { items: [], pageCount: 0 });

export const getCreatures = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<Creatures>("Creatures", page, showBy, { items: [], pageCount: 0 });

export const getDevelopers = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<Developers>("Developers", page, showBy, { items: [], pageCount: 0 });

export const getDeveloperAccessLevels = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<DeveloperAccessLevels>("DeveloperAccessLevels", page, showBy, { items: [], pageCount: 0 });

export const getGameModes = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<GameModes>("GameModes", page, showBy, { items: [], pageCount: 0 });

export const getGameSessions = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<GameSessions>("GameSessions", page, showBy, { items: [], pageCount: 0 });

export const getPlayers = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<Players>("Players", page, showBy, { items: [], pageCount: 0 });

export const getPlayerSessions = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<PlayerSessions>("PlayerSessions", page, showBy, { items: [], pageCount: 0 });

export const getRarityLevels = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<RarityLevels>("RarityLevels", page, showBy, { items: [], pageCount: 0 });

export const getServers = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<Servers>("Servers", page, showBy, { items: [], pageCount: 0 });

export const getXpLevels = async (page?: number | undefined, showBy?: number | undefined) =>
  await getAllWithRelations<XpLevels>("XpLevels", page, showBy, { items: [], pageCount: 0 });

//==================================================================================================

async function getAllWhereDisplayName<TEntities>(controllerUrl: string, displayName: string, page: number | undefined, showBy: number | undefined, empty: TEntities): Promise<TEntities> {
  try {
    const json = `[{ "$type": "binary", "property": "display_name", "operator": "contains substring ignore case", "value": "${displayName}" }]`;
    const response = await axios.post(`${controllerUrl}/GetWhere`, json, {
      headers: {
        Authorization: "Bearer ".concat(await getAccessToken())
      },
      params: {
        page: page,
        showBy: showBy
      }
    });
    return response.data as TEntities;
  }
  catch (error) {
    handleError(error);
    return empty;
  }
}

export const getAbilitiesWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<Abilities>("Abilities", displayName, page, showBy, { items: [], pageCount: 0 });

export const getAcquiredAbilitiesWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<AcquiredAbilities>("AcquiredAbilities", displayName, page, showBy, { items: [], pageCount: 0 });

export const getArtifactsWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<Artifacts>("Artifacts", displayName, page, showBy, { items: [], pageCount: 0 });

export const getCollectedArtifactsWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<CollectedArtifacts>("CollectedArtifacts", displayName, page, showBy, { items: [], pageCount: 0 });

export const getCreaturesWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<Creatures>("Creatures", displayName, page, showBy, { items: [], pageCount: 0 });

export const getDevelopersWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<Developers>("Developers", displayName, page, showBy, { items: [], pageCount: 0 });

export const getDeveloperAccessLevelsWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<DeveloperAccessLevels>("DeveloperAccessLevels", displayName, page, showBy, { items: [], pageCount: 0 });

export const getGameModesWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<GameModes>("GameModes", displayName, page, showBy, { items: [], pageCount: 0 });

export const getGameSessionsWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<GameSessions>("GameSessions", displayName, page, showBy, { items: [], pageCount: 0 });

export const getPlayersWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<Players>("Players", displayName, page, showBy, { items: [], pageCount: 0 });

export const getPlayerSessionsWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<PlayerSessions>("PlayerSessions", displayName, page, showBy, { items: [], pageCount: 0 });

export const getRarityLevelsWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<RarityLevels>("RarityLevels", displayName, page, showBy, { items: [], pageCount: 0 });

export const getServersWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<Servers>("Servers", displayName, page, showBy, { items: [], pageCount: 0 });

export const getXpLevelsWhereDisplayName = async (displayName: string, page: number | undefined, showBy: number | undefined) =>
  await getAllWhereDisplayName<XpLevels>("XpLevels", displayName, page, showBy, { items: [], pageCount: 0 });

//==================================================================================================

async function getWithRelations<TEntity>(controllerUrl: string, id: string | undefined, empty: TEntity): Promise<TEntity> {
  try {
    const response = await axios.get(`${controllerUrl}/GetWithRelations`, {
      headers: {
        Authorization: "Bearer ".concat(await getAccessToken())
      },
      params: {
        id: id
      }
    });
    return response.data as TEntity;
  }
  catch (error) {
    handleError(error);
    return empty;
  }
}

export const getAbility = async (id: string | undefined) =>
  await getWithRelations<Ability>("Abilities", id, new Ability());

export const getAcquiredAbility = async (id: string | undefined) =>
  await getWithRelations<AcquiredAbility>("AcquiredAbilities", id, new AcquiredAbility());

export const getArtifact = async (id: string | undefined) =>
  await getWithRelations<Artifact>("Artifacts", id, new Artifact());

export const getCollectedArtifact = async (id: string | undefined) =>
  await getWithRelations<CollectedArtifact>("CollectedArtifacts", id, new CollectedArtifact());

export const getCreature = async (id: string | undefined) =>
  await getWithRelations<Creature>("Creatures", id, new Creature());

export const getDeveloper = async (id: string | undefined) =>
  await getWithRelations<Developer>("Developers", id, new Developer());

export const getDeveloperAccessLevel = async (id: string | undefined) =>
  await getWithRelations<DeveloperAccessLevel>("DeveloperAccessLevels", id, new DeveloperAccessLevel());

export const getGameMode = async (id: string | undefined) =>
  await getWithRelations<GameMode>("GameModes", id, new GameMode());

export const getGameSession = async (id: string | undefined) =>
  await getWithRelations<GameSession>("GameSessions", id, new GameSession());

export const getPlayer = async (id: string | undefined) =>
  await getWithRelations<Player>("Players", id, new Player());

export const getPlayerSession = async (id: string | undefined) =>
  await getWithRelations<PlayerSession>("PlayerSessions", id, new PlayerSession());

export const getRarityLevel = async (id: string | undefined) =>
  await getWithRelations<RarityLevel>("RarityLevels", id, new RarityLevel());

export const getServer = async (id: string | undefined) =>
  await getWithRelations<Server>("Servers", id, new Server());

export const getXpLevel = async (id: string | undefined) =>
  await getWithRelations<XpLevel>("XpLevels", id, new XpLevel());

//==================================================================================================

async function create<TEntity>(controllerUrl: string, entity: TEntity): Promise<TEntity | undefined> {
  try {
    const response = await axios.post(`${controllerUrl}/Create`, entity, {
      headers: {
        Authorization: "Bearer ".concat(await getAccessToken())
      },
    });
    return response.data as TEntity;
  }
  catch (error) {
    handleError(error);
  }
}

export const createAbility = async (ability: Ability) =>
  await create<Ability>("Abilities", ability);

export const createAcquiredAbility = async (acquiredAbility: AcquiredAbility) =>
  await create<AcquiredAbility>("AcquiredAbilities", acquiredAbility);

export const createArtifact = async (artifact: Artifact) =>
  await create<Artifact>("Artifacts", artifact);

export const createCollectedArtifact = async (collectedArtifact: CollectedArtifact) =>
  await create<CollectedArtifact>("CollectedArtifacts", collectedArtifact);

export const createCreature = async (creature: Creature) =>
  await create<Creature>("Creatures", creature);

export const createDeveloper = async (developer: Developer) =>
  await create<Developer>("Developers", developer);

export const createDeveloperAccessLevel = async (developerAccessLevel: DeveloperAccessLevel) =>
  await create<DeveloperAccessLevel>("DeveloperAccessLevels", developerAccessLevel);

export const createGameMode = async (gameMode: GameMode) =>
  await create<GameMode>("GameModes", gameMode);

export const createGameSession = async (gameSession: GameSession) =>
  await create<GameSession>("GameSessions", gameSession);

export const createPlayer = async (player: Player) =>
  await create<Player>("Players", player);

export const createPlayerSession = async (playerSession: PlayerSession) =>
  await create<PlayerSession>("PlayerSessions", playerSession);

export const createRarityLevel = async (rarityLevel: RarityLevel) =>
  await create<RarityLevel>("RarityLevels", rarityLevel);

export const createServer = async (server: Server) =>
  await create<Server>("Servers", server);

export const createXpLevel = async (xpLevel: XpLevel) =>
  await create<XpLevel>("XpLevels", xpLevel);

//==================================================================================================

async function edit<TEntity>(controllerUrl: string, id: string, entity: TEntity): Promise<TEntity | undefined> {
  try {
    const response = await axios.put(`${controllerUrl}/Edit`, entity, {
      headers: {
        Authorization: "Bearer ".concat(await getAccessToken())
      },
      params: {
        id: id,
      },
    });
    return response.data as TEntity;
  }
  catch (error) {
    handleError(error);
  }
}

export const editAbility = async (ability: Ability) =>
  await edit<Ability>("Abilities", ability.id, ability);

export const editAcquiredAbility = async (acquiredAbility: AcquiredAbility) =>
  await edit<AcquiredAbility>("AcquiredAbilities", acquiredAbility.id, acquiredAbility);

export const editArtifact = async (artifact: Artifact) =>
  await edit<Artifact>("Artifacts", artifact.id, artifact);

export const editCollectedArtifact = async (collectedArtifact: CollectedArtifact) =>
  await edit<CollectedArtifact>("CollectedArtifacts", collectedArtifact.id, collectedArtifact);

export const editCreature = async (creature: Creature) =>
  await edit<Creature>("Creatures", creature.id, creature);

export const editDeveloper = async (developer: Developer) =>
  await edit<Developer>("Developers", developer.id, developer);

export const editDeveloperAccessLevel = async (developerAccessLevel: DeveloperAccessLevel) =>
  await edit<DeveloperAccessLevel>("DeveloperAccessLevels", developerAccessLevel.id, developerAccessLevel);

export const editGameMode = async (gameMode: GameMode) =>
  await edit<GameMode>("GameModes", gameMode.id, gameMode);

export const editGameSession = async (gameSession: GameSession) =>
  await edit<GameSession>("GameSessions", gameSession.id, gameSession);

export const editPlayer = async (player: Player) =>
  await edit<Player>("Players", player.id, player);

export const editPlayerSession = async (playerSession: PlayerSession) =>
  await edit<PlayerSession>("PlayerSessions", playerSession.id, playerSession);

export const editRarityLevel = async (rarityLevel: RarityLevel) =>
  await edit<RarityLevel>("RarityLevels", rarityLevel.id, rarityLevel);

export const editServer = async (server: Server) =>
  await edit<Server>("Servers", server.id, server);

export const editXpLevel = async (xpLevel: XpLevel) =>
  await edit<XpLevel>("XpLevels", xpLevel.id, xpLevel);

//==================================================================================================

async function delete_(controllerUrl: string, id: string): Promise<boolean> {
  try {
    await axios.delete(`${controllerUrl}/Delete`, {
      headers: {
        Authorization: "Bearer ".concat(await getAccessToken())
      },
      params: {
        id: id,
      },
    });
    return true;
  }
  catch (error) {
    handleError(error);
    return false;
  }
}

export const deleteAbility = async (id: string) =>
  await delete_("Abilities", id);

export const deleteAcquiredAbility = async (id: string) =>
  await delete_("AcquiredAbilities", id);

export const deleteArtifact = async (id: string) =>
  await delete_("Artifacts", id);

export const deleteCollectedArtifact = async (id: string) =>
  await delete_("CollectedArtifacts", id);

export const deleteCreature = async (id: string) =>
  await delete_("Creatures", id);

export const deleteDeveloper = async (id: string) =>
  await delete_("Developers", id);

export const deleteDeveloperAccessLevel = async (id: string) =>
  await delete_("DeveloperAccessLevels", id);

export const deleteGameMode = async (id: string) =>
  await delete_("GameModes", id);

export const deleteGameSession = async (id: string) =>
  await delete_("GameSessions", id);

export const deletePlayer = async (id: string) =>
  await delete_("Players", id);

export const deletePlayerSession = async (id: string) =>
  await delete_("PlayerSessions", id);

export const deleteRarityLevel = async (id: string) =>
  await delete_("RarityLevels", id);

export const deleteServer = async (id: string) =>
  await delete_("Servers", id);

export const deleteXpLevel = async (id: string) =>
  await delete_("XpLevels", id);
