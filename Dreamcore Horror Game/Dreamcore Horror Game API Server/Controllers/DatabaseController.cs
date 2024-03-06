using Dreamcore_Horror_Game_API_Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dreamcore_Horror_Game_API_Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DatabaseController : ControllerBase
    {
        private readonly DreamcoreHorrorGameContext _context;

        private const string ENTITY_SET_IS_NULL = "Requested entity set is null.";
        private const string ID_DOES_NOT_MATCH = "Received parameter 'id' does not match the 'id' value of the object.";
        private const string INVALID_ENTITY_DATA = "Invalid entity data.";

        public DatabaseController(DreamcoreHorrorGameContext context)
        {
            _context = context;
        }

        // POST note:
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        #region Abilities

        [HttpGet]
        public async Task<IActionResult> GetAbilities()
        {
            return _context.Abilities == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Abilities.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAbility(Guid? id)
        {
            if (id == null || _context.Abilities == null)
                return NotFound();

            var ability = await _context.Abilities.FindAsync(id);

            if (ability == null)
                return NotFound();

            return Ok(ability);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAbility([Bind("Id,AssetName")] Ability ability)
        {
            if (ModelState.IsValid)
            {
                ability.Id = Guid.NewGuid();
                _context.Add(ability);
                await _context.SaveChangesAsync();
                return Ok(ability);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAbility(Guid? id, [Bind("Id,AssetName")] Ability ability)
        {
            if (id == null || _context.Abilities == null)
                return NotFound();

            if (id != ability.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AbilityExists(ability.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(ability);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAbility(Guid? id)
        {
            if (id == null || _context.Abilities == null)
                return NotFound();

            var ability = await _context.Abilities.FindAsync(id);

            if (ability == null)
                return NotFound();

            _context.Abilities.Remove(ability);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool AbilityExists(Guid id) => (_context.Abilities?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Acquired Abilities

        [HttpGet]
        public async Task<IActionResult> GetAcquiredAbilities()
        {
            return _context.AcquiredAbilities == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.AcquiredAbilities.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetAcquiredAbility(Guid? id)
        {
            if (id == null || _context.AcquiredAbilities == null)
                return NotFound();

            var acquiredAbility = await _context.AcquiredAbilities.FindAsync(id);

            if (acquiredAbility == null)
                return NotFound();

            return Ok(acquiredAbility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAcquiredAbility([Bind("Id,PlayerId,AbilityId,AcquirementTimestamp")] AcquiredAbility acquiredAbility)
        {
            if (ModelState.IsValid)
            {
                acquiredAbility.Id = Guid.NewGuid();
                _context.Add(acquiredAbility);
                await _context.SaveChangesAsync();
                return Ok(acquiredAbility);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAcquiredAbility(Guid? id, [Bind("Id,PlayerId,AbilityId,AcquirementTimestamp")] AcquiredAbility acquiredAbility)
        {
            if (id == null || _context.AcquiredAbilities == null)
                return NotFound();

            if (id != acquiredAbility.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(acquiredAbility);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcquiredAbilityExists(acquiredAbility.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(acquiredAbility);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAcquiredAbility(Guid? id)
        {
            if (id == null || _context.AcquiredAbilities == null)
                return NotFound();

            var acquiredAbility = await _context.AcquiredAbilities.FindAsync(id);

            if (acquiredAbility == null)
                return NotFound();

            _context.AcquiredAbilities.Remove(acquiredAbility);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool AcquiredAbilityExists(Guid id) => (_context.AcquiredAbilities?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Artifacts

        [HttpGet]
        public async Task<IActionResult> GetArtifacts()
        {
            return _context.Artifacts == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Artifacts.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetArtifact(Guid? id)
        {
            if (id == null || _context.Artifacts == null)
                return NotFound();

            var artifact = await _context.Artifacts.FindAsync(id);

            if (artifact == null)
                return NotFound();

            return Ok(artifact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArtifact([Bind("Id,AssetName,RarityLevelId")] Artifact artifact)
        {
            if (ModelState.IsValid)
            {
                artifact.Id = Guid.NewGuid();
                _context.Add(artifact);
                await _context.SaveChangesAsync();
                return Ok(artifact);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArtifact(Guid? id, [Bind("Id,AssetName,RarityLevelId")] Artifact artifact)
        {
            if (id == null || _context.Artifacts == null)
                return NotFound();

            if (id != artifact.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artifact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtifactExists(artifact.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(artifact);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteArtifact(Guid? id)
        {
            if (id == null || _context.Artifacts == null)
                return NotFound();

            var artifact = await _context.Artifacts.FindAsync(id);

            if (artifact == null)
                return NotFound();

            _context.Artifacts.Remove(artifact);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ArtifactExists(Guid id) => (_context.Artifacts?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Collected Artifacts

        [HttpGet]
        public async Task<IActionResult> GetCollectedArtifacts()
        {
            return _context.CollectedArtifacts == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.CollectedArtifacts.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCollectedArtifact(Guid? id)
        {
            if (id == null || _context.CollectedArtifacts == null)
                return NotFound();

            var collectedArtifact = await _context.CollectedArtifacts.FindAsync(id);

            if (collectedArtifact == null)
                return NotFound();

            return Ok(collectedArtifact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCollectedArtifact([Bind("Id,PlayerId,ArtifactId,CollectionTimestamp")] CollectedArtifact collectedArtifact)
        {
            if (ModelState.IsValid)
            {
                collectedArtifact.Id = Guid.NewGuid();
                _context.Add(collectedArtifact);
                await _context.SaveChangesAsync();
                return Ok(collectedArtifact);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCollectedArtifact(Guid? id, [Bind("Id,PlayerId,ArtifactId,CollectionTimestamp")] CollectedArtifact collectedArtifact)
        {
            if (id == null || _context.CollectedArtifacts == null)
                return NotFound();

            if (id != collectedArtifact.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(collectedArtifact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollectedArtifactExists(collectedArtifact.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(collectedArtifact);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCollectedArtifact(Guid? id)
        {
            if (id == null || _context.CollectedArtifacts == null)
                return NotFound();

            var collectedArtifact = await _context.CollectedArtifacts.FindAsync(id);

            if (collectedArtifact == null)
                return NotFound();

            _context.CollectedArtifacts.Remove(collectedArtifact);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool CollectedArtifactExists(Guid id) => (_context.CollectedArtifacts?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Creatures

        [HttpGet]
        public async Task<IActionResult> GetCreatures()
        {
            return _context.Creatures == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Creatures.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetCreature(Guid? id)
        {
            if (id == null || _context.Creatures == null)
                return NotFound();

            var creature = await _context.Creatures.FindAsync(id);

            if (creature == null)
                return NotFound();

            return Ok(creature);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCreature([Bind("Id,AssetName,RequiredExperienceLevelId,Health,MovementSpeed")] Creature creature)
        {
            if (ModelState.IsValid)
            {
                creature.Id = Guid.NewGuid();
                _context.Add(creature);
                await _context.SaveChangesAsync();
                return Ok(creature);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCreature(Guid? id, [Bind("Id,AssetName,RequiredExperienceLevelId,Health,MovementSpeed")] Creature creature)
        {
            if (id == null || _context.Creatures == null)
                return NotFound();

            if (id != creature.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(creature);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CreatureExists(creature.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(creature);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCreature(Guid? id)
        {
            if (id == null || _context.Creatures == null)
                return NotFound();

            var creature = await _context.Creatures.FindAsync(id);

            if (creature == null)
                return NotFound();

            _context.Creatures.Remove(creature);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool CreatureExists(Guid id) => (_context.Creatures?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Experience Levels

        [HttpGet]
        public async Task<IActionResult> GetExperienceLevels()
        {
            return _context.ExperienceLevels == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.ExperienceLevels.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetExperienceLevel(Guid? id)
        {
            if (id == null || _context.ExperienceLevels == null)
                return NotFound();

            var experienceLevel = await _context.ExperienceLevels.FindAsync(id);

            if (experienceLevel == null)
                return NotFound();

            return Ok(experienceLevel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExperienceLevel([Bind("Id,Number,RequiredExperiencePoints")] ExperienceLevel experienceLevel)
        {
            if (ModelState.IsValid)
            {
                experienceLevel.Id = Guid.NewGuid();
                _context.Add(experienceLevel);
                await _context.SaveChangesAsync();
                return Ok(experienceLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditExperienceLevel(Guid? id, [Bind("Id,Number,RequiredExperiencePoints")] ExperienceLevel experienceLevel)
        {
            if (id == null || _context.ExperienceLevels == null)
                return NotFound();

            if (id != experienceLevel.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(experienceLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExperienceLevelExists(experienceLevel.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(experienceLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteExperienceLevel(Guid? id)
        {
            if (id == null || _context.ExperienceLevels == null)
                return NotFound();

            var experienceLevel = await _context.ExperienceLevels.FindAsync(id);

            if (experienceLevel == null)
                return NotFound();

            _context.ExperienceLevels.Remove(experienceLevel);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ExperienceLevelExists(Guid id) => (_context.ExperienceLevels?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Game Modes

        [HttpGet]
        public async Task<IActionResult> GetGameModes()
        {
            return _context.GameModes == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.GameModes.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetGameMode(Guid? id)
        {
            if (id == null || _context.GameModes == null)
                return NotFound();

            var gameMode = await _context.GameModes.FindAsync(id);

            if (gameMode == null)
                return NotFound();

            return Ok(gameMode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGameMode([Bind("Id,AssetName,MaxPlayers,TimeLimit,IsActive")] GameMode gameMode)
        {
            if (ModelState.IsValid)
            {
                gameMode.Id = Guid.NewGuid();
                _context.Add(gameMode);
                await _context.SaveChangesAsync();
                return Ok(gameMode);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGameMode(Guid? id, [Bind("Id,AssetName,MaxPlayers,TimeLimit,IsActive")] GameMode gameMode)
        {
            if (id == null || _context.GameModes == null)
                return NotFound();

            if (id != gameMode.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameMode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameModeExists(gameMode.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(gameMode);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGameMode(Guid? id)
        {
            if (id == null || _context.GameModes == null)
                return NotFound();

            var gameMode = await _context.GameModes.FindAsync(id);

            if (gameMode == null)
                return NotFound();

            _context.GameModes.Remove(gameMode);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool GameModeExists(Guid id) => (_context.GameModes?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Game Sessions

        [HttpGet]
        public async Task<IActionResult> GetGameSessions()
        {
            return _context.GameSessions == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.GameSessions.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetGameSession(Guid? id)
        {
            if (id == null || _context.GameSessions == null)
                return NotFound();

            var gameSession = await _context.GameSessions.FindAsync(id);

            if (gameSession == null)
                return NotFound();

            return Ok(gameSession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGameSession([Bind("Id,ServerId,GameModeId,StartTimestamp,EndTimestamp")] GameSession gameSession)
        {
            if (ModelState.IsValid)
            {
                gameSession.Id = Guid.NewGuid();
                _context.Add(gameSession);
                await _context.SaveChangesAsync();
                return Ok(gameSession);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGameSession(Guid? id, [Bind("Id,ServerId,GameModeId,StartTimestamp,EndTimestamp")] GameSession gameSession)
        {
            if (id == null || _context.GameSessions == null)
                return NotFound();

            if (id != gameSession.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gameSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameSessionExists(gameSession.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(gameSession);
            }
            
            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGameSession(Guid? id)
        {
            if (id == null || _context.GameSessions == null)
                return NotFound();

            var gameSession = await _context.GameSessions.FindAsync(id);

            if (gameSession == null)
                return NotFound();

            _context.GameSessions.Remove(gameSession);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool GameSessionExists(Guid id) => (_context.GameSessions?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Players

        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            return _context.Players == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Players.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayer(Guid? id)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return NotFound();

            return Ok(player);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlayer([Bind("Id,Username,Email,Password,RegistrationTimestamp,CollectOptionalData,IsOnline,ExperienceLevelId,ExperiencePoints,AbilityPoints,SpiritEnergyPoints")] Player player)
        {
            if (ModelState.IsValid)
            {
                player.Id = Guid.NewGuid();
                _context.Add(player);
                await _context.SaveChangesAsync();
                return Ok(player);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlayer(Guid? id, [Bind("Id,Username,Email,Password,RegistrationTimestamp,CollectOptionalData,IsOnline,ExperienceLevelId,ExperiencePoints,AbilityPoints,SpiritEnergyPoints")] Player player)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            if (id != player.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(player);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlayer(Guid? id)
        {
            if (id == null || _context.Players == null)
                return NotFound();

            var player = await _context.Players.FindAsync(id);

            if (player == null)
                return NotFound();

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool PlayerExists(Guid id) => (_context.Players?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Player Sessions

        [HttpGet]
        public async Task<IActionResult> GetPlayerSessions()
        {
            return _context.PlayerSessions == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.PlayerSessions.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayerSession(Guid? id)
        {
            if (id == null || _context.PlayerSessions == null)
                return NotFound();

            var playerSession = await _context.PlayerSessions.FindAsync(id);

            if (playerSession == null)
                return NotFound();

            return Ok(playerSession);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlayerSession([Bind("Id,GameSessionId,PlayerId,StartTimestamp,EndTimestamp,IsCompleted,IsWon,TimeAlive,PlayedAsCreature,UsedCreatureId")] PlayerSession playerSession)
        {
            if (ModelState.IsValid)
            {
                playerSession.Id = Guid.NewGuid();
                _context.Add(playerSession);
                await _context.SaveChangesAsync();
                return Ok(playerSession);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlayerSession(Guid? id, [Bind("Id,GameSessionId,PlayerId,StartTimestamp,EndTimestamp,IsCompleted,IsWon,TimeAlive,PlayedAsCreature,UsedCreatureId")] PlayerSession playerSession)
        {
            if (id == null || _context.PlayerSessions == null)
                return NotFound();

            if (id != playerSession.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playerSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerSessionExists(playerSession.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(playerSession);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlayerSession(Guid? id)
        {
            if (id == null || _context.PlayerSessions == null)
                return NotFound();

            var playerSession = await _context.PlayerSessions.FindAsync(id);

            if (playerSession == null)
                return NotFound();

            _context.PlayerSessions.Remove(playerSession);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool PlayerSessionExists(Guid id) => (_context.PlayerSessions?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Rarity Levels

        [HttpGet]
        public async Task<IActionResult> GetRarityLevels()
        {
            return _context.RarityLevels == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.RarityLevels.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetRarityLevel(Guid? id)
        {
            if (id == null || _context.RarityLevels == null)
                return NotFound();

            var rarityLevel = await _context.RarityLevels.FindAsync(id);

            if (rarityLevel == null)
                return NotFound();

            return Ok(rarityLevel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRarityLevel([Bind("Id,AssetName,Probability")] RarityLevel rarityLevel)
        {
            if (ModelState.IsValid)
            {
                rarityLevel.Id = Guid.NewGuid();
                _context.Add(rarityLevel);
                await _context.SaveChangesAsync();
                return Ok(rarityLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRarityLevel(Guid? id, [Bind("Id,AssetName,Probability")] RarityLevel rarityLevel)
        {
            if (id == null || _context.RarityLevels == null)
                return NotFound();

            if (id != rarityLevel.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rarityLevel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RarityLevelExists(rarityLevel.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(rarityLevel);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRarityLevel(Guid? id)
        {
            if (id == null || _context.RarityLevels == null)
                return NotFound();

            var rarityLevel = await _context.RarityLevels.FindAsync(id);

            if (rarityLevel == null)
                return NotFound();

            _context.RarityLevels.Remove(rarityLevel);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool RarityLevelExists(Guid id) => (_context.RarityLevels?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion

        #region Servers

        [HttpGet]
        public async Task<IActionResult> GetServers()
        {
            return _context.Servers == null ?
                Problem(ENTITY_SET_IS_NULL) :
                Ok(await _context.Servers.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetServer(Guid? id)
        {
            if (id == null || _context.Servers == null)
                return NotFound();

            var server = await _context.Servers.FindAsync(id);

            if (server == null)
                return NotFound();

            return Ok(server);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateServer([Bind("Id,IpAddress,PlayerCapacity,IsOnline")] Server server)
        {
            if (ModelState.IsValid)
            {
                server.Id = Guid.NewGuid();
                _context.Add(server);
                await _context.SaveChangesAsync();
                return Ok(server);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditServer(Guid? id, [Bind("Id,IpAddress,PlayerCapacity,IsOnline")] Server server)
        {
            if (id == null || _context.Servers == null)
                return NotFound();

            if (id != server.Id)
                return BadRequest(ID_DOES_NOT_MATCH);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(server);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServerExists(server.Id))
                        return NotFound();
                    else
                        throw;
                }
                return Ok(server);
            }

            return BadRequest(INVALID_ENTITY_DATA);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteServer(Guid? id)
        {
            if (id == null || _context.Servers == null)
                return NotFound();

            var server = await _context.Servers.FindAsync(id);

            if (server == null)
                return NotFound();

            _context.Servers.Remove(server);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool ServerExists(Guid id) => (_context.Servers?.Any(x => x.Id == id)).GetValueOrDefault();

        #endregion
    }
}
