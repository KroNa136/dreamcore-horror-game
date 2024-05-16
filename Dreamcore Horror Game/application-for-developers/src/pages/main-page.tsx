import * as React from "react";
import CssBaseline from "@mui/material/CssBaseline";
import Grid from "@mui/material/Grid";
import Container from "@mui/material/Container";
import { ThemeProvider } from "@mui/material/styles";
import Header from "../components/header";
import TableCard from "../components/cards/table-card";
import Footer from "../components/footer";
import { defaultTheme } from "../themes";
import { Backdrop, Box, Button, Fade, Modal, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import {
  getAbilityCount, getAcquiredAbilityCount, getArtifactCount, getCollectedArtifactCount, getCreatureCount, getDeveloperCount,
  getDeveloperAccessLevelCount, getGameModeCount, getGameSessionCount, getPlayerCount, getPlayerSessionCount, getRarityLevelCount,
  getServerCount, getXpLevelCount
} from "../requests";
import { canViewDevelopmentTables } from "../auth-state";

export default function MainPage() {
  const [iconsModalOpen, setIconsModalOpen] = useState<boolean>(false);

  const [abilityCount, setAbilityCount] = useState<number>(0);
  const [acquiredAbilityCount, setAcquiredAbilityCount] = useState<number>(0);
  const [artifactCount, setArtifactCount] = useState<number>(0);
  const [collectedArtifactCount, setCollectedArtifactCount] = useState<number>(0);
  const [creatureCount, setCreatureCount] = useState<number>(0);
  const [developerCount, setDeveloperCount] = useState<number>(0);
  const [developerAccessLevelCount, setDeveloperAccessLevelCount] = useState<number>(0);
  const [gameModeCount, setGameModeCount] = useState<number>(0);
  const [gameSessionCount, setGameSessionCount] = useState<number>(0);
  const [playerCount, setPlayerCount] = useState<number>(0);
  const [playerSessionCount, setPlayerSessionCount] = useState<number>(0);
  const [rarityLevelCount, setRarityLevelCount] = useState<number>(0);
  const [serverCount, setServerCount] = useState<number>(0);
  const [xpLevelCount, setXpLevelCount] = useState<number>(0);

  useEffect(() => {
    getAbilityCount().then(count => setAbilityCount(count));
    getAcquiredAbilityCount().then(count => setAcquiredAbilityCount(count));
    getArtifactCount().then(count => setArtifactCount(count));
    getCollectedArtifactCount().then(count => setCollectedArtifactCount(count));
    getCreatureCount().then(count => setCreatureCount(count));
    getDeveloperCount().then(count => setDeveloperCount(count));
    getDeveloperAccessLevelCount().then(count => setDeveloperAccessLevelCount(count));
    getGameModeCount().then(count => setGameModeCount(count));
    getGameSessionCount().then(count => setGameSessionCount(count));
    getPlayerCount().then(count => setPlayerCount(count));
    getPlayerSessionCount().then(count => setPlayerSessionCount(count));
    getRarityLevelCount().then(count => setRarityLevelCount(count));
    getServerCount().then(count => setServerCount(count));
    getXpLevelCount().then(count => setXpLevelCount(count));
  });

  const gameContentTables = [
    {
      title: "Способности",
      databaseTitle: "abilities",
      entryCount: abilityCount,
      image: "/table_icons/abilities.png",
      imageLabel: "Способности",
      link: "/abilities",
    },
    {
      title: "Артефакты",
      databaseTitle: "artifacts",
      entryCount: artifactCount,
      image: "/table_icons/artifacts.png",
      imageLabel: "Артефакты",
      link: "/artifacts",
    },
    {
      title: "Существа",
      databaseTitle: "creatures",
      entryCount: creatureCount,
      image: "/table_icons/creatures.png",
      imageLabel: "Существа",
      link: "/creatures",
    },
    {
      title: "Игровые режимы",
      databaseTitle: "game_modes",
      entryCount: gameModeCount,
      image: "/table_icons/game_modes.png",
      imageLabel: "Игровые режимы",
      link: "/gameModes",
    },
    {
      title: "Уровни редкости",
      databaseTitle: "rarity_levels",
      entryCount: rarityLevelCount,
      image: "/table_icons/rarity_levels.png",
      imageLabel: "Уровни редкости",
      link: "/rarityLevels",
    },
    {
      title: "Уровни опыта",
      databaseTitle: "xp_levels",
      entryCount: xpLevelCount,
      image: "/table_icons/xp_levels.png",
      imageLabel: "Уровни опыта",
      link: "/xpLevels",
    },
  ];

  const onlineGameTables = [
    {
      title: "Приобретённые способности",
      databaseTitle: "acquired_abilities",
      entryCount: acquiredAbilityCount,
      image: "/table_icons/acquired_abilities.png",
      imageLabel: "Приобретённые способности",
      link: "/acquiredAbilities",
    },
    {
      title: "Подобранные артефакты",
      databaseTitle: "collected_artifacts",
      entryCount: collectedArtifactCount,
      image: "/table_icons/collected_artifacts.png",
      imageLabel: "Подобранные артефакты",
      link: "/collectedArtifacts",
    },
    {
      title: "Игровые сеансы",
      databaseTitle: "game_sessions",
      entryCount: gameSessionCount,
      image: "/table_icons/game_sessions.png",
      imageLabel: "Игровые сеансы",
      link: "/gameSessions",
    },
    {
      title: "Сеансы игроков",
      databaseTitle: "player_sessions",
      entryCount: playerSessionCount,
      image: "/table_icons/player_sessions.png",
      imageLabel: "Сеансы игроков",
      link: "/playerSessions",
    },
    {
      title: "Игроки",
      databaseTitle: "players",
      entryCount: playerCount,
      image: "/table_icons/players.png",
      imageLabel: "Игроки",
      link: "/players",
    },
    {
      title: "Серверы",
      databaseTitle: "servers",
      entryCount: serverCount,
      image: "/table_icons/servers.png",
      imageLabel: "Серверы",
      link: "/servers",
    },
  ];

  const developerTables = [
    {
      title: "Уровни доступа разработчиков",
      databaseTitle: "developer_access_levels",
      entryCount: developerAccessLevelCount,
      image: "/table_icons/developer_access_levels.png",
      imageLabel: "Уровни доступа разработчиков",
      link: "/developerAccessLevels",
    },
    {
      title: "Разработчики",
      databaseTitle: "developers",
      entryCount: developerCount,
      image: "/table_icons/developers.png",
      imageLabel: "Разработчики",
      link: "/developers",
    },
  ];

  return (
    <ThemeProvider theme={defaultTheme}>
      <CssBaseline />
      <Container maxWidth="lg">
        <Header />
        <main>
          <div>
            <Typography component="h4" variant="h3" align="left" my={4}>Игровой контент</Typography>
            <Grid container spacing={4}>
              {gameContentTables.map((table) => (
                <TableCard key={table.title} table={table} />
              ))}
            </Grid>
          </div>
          <div>
            <Typography component="h4" variant="h3" align="left" my={4}>Сетевая игра</Typography>
            <Grid container spacing={4}>
              {onlineGameTables.map((table) => (
                <TableCard key={table.title} table={table} />
              ))}
            </Grid>
          </div>
          {canViewDevelopmentTables() &&
            <div>
              <Typography component="h3" variant="h3" align="left" my={4}>Разработка</Typography>
              <Grid container spacing={4}>
                {developerTables.map((table) => (
                  <TableCard key={table.title} table={table} />
                ))}
              </Grid>
            </div>
          }
          <Grid container spacing={4} direction="row" justifyContent="center" alignItems="center" my={4} px={2}>
            <Button size="medium" variant="contained" color="primary" sx={{ mx: 1 }} onClick={() => setIconsModalOpen(true)}>Использованные иконки</Button>
          </Grid>
        </main>
        <Footer />
      </Container>
      <Modal
        open={iconsModalOpen}
        onClose={() => setIconsModalOpen(false)}
        aria-labelledby="modal-modal-title"
        aria-describedby="modal-modal-description"
        closeAfterTransition
        slots={{ backdrop: Backdrop }}
        slotProps={{
          backdrop: {
            timeout: 200,
          },
        }}
      >
        <Fade in={iconsModalOpen}>
          <Box sx={{
            position: "absolute" as "absolute",
            top: "50%",
            left: "50%",
            transform: "translate(-50%, -50%)",
            width: 480,
            bgcolor: "background.paper",
            boxShadow: 24,
            p: 4,
          }}>
            <Typography id="modal-modal-title" variant="h6" component="h2">
              Использованные иконки
            </Typography>
            <Grid container direction="column">
              <ul>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/hbSnr96BuuQG/abilities">Abilities</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/upUDqxuJWw2B/archeology">Archeology</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/15784/tentacles">Tentacles</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/21636/adjust">Level</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/2NE7EIbky0uS/probability">Probability</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/MAuvVb8zy9c6/stars">Stars</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/E8p9kd1wRyug/player">player</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/y91Uh9uvUNf3/session-timeout">Session Timeout</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/7314/game-controller">Game</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/1340/server">Server</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/30694/access">Access</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
                <Grid item component="li">
                  <a target="_blank" href="https://icons8.com/icon/rpWJYJBNjJbi/dev-community-where-programmers-share-ideas-and-help-each-other-grow">Dev community where programmers share ideas and help each other grow</a> с сайта <a target="_blank" href="https://icons8.com">Icons8</a>
                </Grid>
              </ul>
            </Grid>
          </Box>
        </Fade>
      </Modal>
    </ThemeProvider>
  );
}