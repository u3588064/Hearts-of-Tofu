/**
 * 极简兵棋 - 世界征服
 * Web Demo for Quest 2 Wargame
 */

// ===== Game Configuration =====
const CONFIG = {
    // Starting points range (random per territory)
    startingPointsMin: 15,
    startingPointsMax: 30,
    // Growth rate range (random per territory)
    growthRateMin: 0.5,
    growthRateMax: 1.5,
    maxPoints: 100,
    minAttackPoints: 5,
    aiDecisionInterval: 2000,
    aiAggressiveness: 0.6,
    gameSpeed: 1
};

// ===== Faction Colors =====
const FACTION_COLORS = [
    '#4a9eff',  // Blue - Player
    '#ff4a4a',  // Red
    '#4aff7a',  // Green
    '#ffcc4a',  // Yellow
    '#aa4aff',  // Purple
    '#ff7a4a'   // Orange
];

const FACTION_NAMES = ['联盟', '帝国', '共和', '联邦', '王国', '自由军'];

// ===== World Map Data =====
const WORLD_TERRITORIES = [
    // North America
    { id: 'alaska', name: '阿拉斯加', region: '北美', x: 120, y: 80, neighbors: ['canada', 'siberia'] },
    { id: 'canada', name: '加拿大', region: '北美', x: 200, y: 90, neighbors: ['alaska', 'usa_west', 'usa_east', 'greenland'] },
    { id: 'greenland', name: '格陵兰', region: '北美', x: 350, y: 60, neighbors: ['canada', 'north_europe'] },
    { id: 'usa_west', name: '美国西部', region: '北美', x: 150, y: 160, neighbors: ['canada', 'usa_east', 'mexico'] },
    { id: 'usa_east', name: '美国东部', region: '北美', x: 230, y: 170, neighbors: ['canada', 'usa_west', 'mexico', 'caribbean'] },
    { id: 'mexico', name: '墨西哥', region: '北美', x: 170, y: 230, neighbors: ['usa_west', 'usa_east', 'central_america'] },
    { id: 'central_america', name: '中美洲', region: '北美', x: 210, y: 280, neighbors: ['mexico', 'caribbean', 'colombia'] },
    { id: 'caribbean', name: '加勒比', region: '北美', x: 270, y: 260, neighbors: ['usa_east', 'central_america', 'colombia'] },

    // South America
    { id: 'colombia', name: '哥伦比亚', region: '南美', x: 240, y: 320, neighbors: ['central_america', 'caribbean', 'brazil', 'andes'] },
    { id: 'brazil', name: '巴西', region: '南美', x: 310, y: 380, neighbors: ['colombia', 'andes', 'south_cone'] },
    { id: 'andes', name: '安第斯', region: '南美', x: 230, y: 400, neighbors: ['colombia', 'brazil', 'south_cone'] },
    { id: 'south_cone', name: '南锥体', region: '南美', x: 270, y: 460, neighbors: ['brazil', 'andes'] },

    // Europe
    { id: 'north_europe', name: '北欧', region: '欧洲', x: 480, y: 70, neighbors: ['greenland', 'west_europe', 'central_europe', 'russia_west'] },
    { id: 'west_europe', name: '西欧', region: '欧洲', x: 450, y: 140, neighbors: ['north_europe', 'iberia', 'central_europe', 'north_africa'] },
    { id: 'iberia', name: '伊比利亚', region: '欧洲', x: 420, y: 180, neighbors: ['west_europe', 'north_africa'] },
    { id: 'central_europe', name: '中欧', region: '欧洲', x: 510, y: 130, neighbors: ['north_europe', 'west_europe', 'italy', 'east_europe', 'balkans'] },
    { id: 'italy', name: '意大利', region: '欧洲', x: 500, y: 175, neighbors: ['central_europe', 'balkans', 'north_africa'] },
    { id: 'east_europe', name: '东欧', region: '欧洲', x: 560, y: 110, neighbors: ['central_europe', 'balkans', 'russia_west'] },
    { id: 'balkans', name: '巴尔干', region: '欧洲', x: 540, y: 170, neighbors: ['central_europe', 'italy', 'east_europe', 'middle_east'] },
    { id: 'russia_west', name: '俄罗斯西部', region: '欧洲', x: 620, y: 90, neighbors: ['north_europe', 'east_europe', 'central_asia', 'siberia'] },

    // Asia
    { id: 'siberia', name: '西伯利亚', region: '亚洲', x: 750, y: 60, neighbors: ['alaska', 'russia_west', 'central_asia', 'china_north', 'far_east'] },
    { id: 'central_asia', name: '中亚', region: '亚洲', x: 680, y: 160, neighbors: ['russia_west', 'siberia', 'middle_east', 'south_asia', 'china_north'] },
    { id: 'middle_east', name: '中东', region: '亚洲', x: 590, y: 220, neighbors: ['balkans', 'central_asia', 'south_asia', 'north_africa', 'east_africa'] },
    { id: 'south_asia', name: '南亚', region: '亚洲', x: 700, y: 260, neighbors: ['central_asia', 'middle_east', 'southeast_asia'] },
    { id: 'china_north', name: '中国', region: '亚洲', x: 780, y: 170, neighbors: ['siberia', 'central_asia', 'south_asia', 'southeast_asia', 'japan_korea', 'far_east'] },
    { id: 'far_east', name: '远东', region: '亚洲', x: 870, y: 90, neighbors: ['siberia', 'china_north', 'japan_korea'] },
    { id: 'japan_korea', name: '日韩', region: '亚洲', x: 870, y: 160, neighbors: ['far_east', 'china_north'] },
    { id: 'southeast_asia', name: '东南亚', region: '亚洲', x: 790, y: 300, neighbors: ['south_asia', 'china_north', 'australia'] },

    // Africa
    { id: 'north_africa', name: '北非', region: '非洲', x: 480, y: 240, neighbors: ['iberia', 'west_europe', 'italy', 'middle_east', 'west_africa', 'east_africa'] },
    { id: 'west_africa', name: '西非', region: '非洲', x: 440, y: 320, neighbors: ['north_africa', 'east_africa', 'south_africa'] },
    { id: 'east_africa', name: '东非', region: '非洲', x: 560, y: 340, neighbors: ['north_africa', 'middle_east', 'west_africa', 'south_africa'] },
    { id: 'south_africa', name: '南非', region: '非洲', x: 520, y: 420, neighbors: ['west_africa', 'east_africa'] },

    // Oceania
    { id: 'australia', name: '澳大利亚', region: '大洋洲', x: 850, y: 400, neighbors: ['southeast_asia', 'pacific'] },
    { id: 'pacific', name: '太平洋岛屿', region: '大洋洲', x: 940, y: 340, neighbors: ['australia', 'japan_korea'] }
];

// ===== Game State =====
let gameState = {
    territories: {},
    factions: [],
    selectedTerritory: null,
    targetTerritory: null,
    isPaused: false,
    turn: 1,
    gameOver: false,
    playerFactionId: 0
};

let gameLoopInterval = null;
let aiIntervals = [];

// ===== Initialize Game =====
function initGame(factionCount = 4) {
    // Reset state
    gameState = {
        territories: {},
        factions: [],
        selectedTerritory: null,
        targetTerritory: null,
        isPaused: false,
        turn: 1,
        gameOver: false,
        playerFactionId: 0
    };

    // Clear intervals
    if (gameLoopInterval) clearInterval(gameLoopInterval);
    aiIntervals.forEach(interval => clearInterval(interval));
    aiIntervals = [];

    // Initialize factions
    for (let i = 0; i < factionCount; i++) {
        gameState.factions.push({
            id: i,
            name: FACTION_NAMES[i],
            color: FACTION_COLORS[i],
            isPlayer: i === 0,
            isEliminated: false
        });
    }

    // Initialize territories with random starting points and growth rates
    WORLD_TERRITORIES.forEach(t => {
        const startingPoints = Math.floor(
            CONFIG.startingPointsMin + Math.random() * (CONFIG.startingPointsMax - CONFIG.startingPointsMin)
        );
        const growthRate =
            CONFIG.growthRateMin + Math.random() * (CONFIG.growthRateMax - CONFIG.growthRateMin);
        gameState.territories[t.id] = {
            ...t,
            points: startingPoints,
            growthRate: growthRate,
            maxPoints: CONFIG.maxPoints,
            owner: null
        };
    });

    // Assign territories to factions
    assignTerritories(factionCount);

    // Render map
    renderMap();
    updateUI();

    // Start game loop
    startGameLoop();

    // Start AI
    startAI();

    addLog('游戏开始！消灭所有敌人获得胜利！', 'info');
}

// ===== Assign Initial Territories =====
function assignTerritories(factionCount) {
    const territoryIds = Object.keys(gameState.territories);
    const shuffled = territoryIds.sort(() => Math.random() - 0.5);
    const perFaction = Math.floor(shuffled.length / factionCount);

    let index = 0;
    for (let i = 0; i < factionCount; i++) {
        for (let j = 0; j < perFaction && index < shuffled.length; j++) {
            gameState.territories[shuffled[index]].owner = i;
            index++;
        }
    }

    // Remaining territories go to first faction
    while (index < shuffled.length) {
        gameState.territories[shuffled[index]].owner = 0;
        index++;
    }
}

// ===== Render Map =====
function renderMap() {
    const svg = document.getElementById('map-svg');
    svg.innerHTML = '';

    // Draw connection lines first
    const drawnConnections = new Set();
    Object.values(gameState.territories).forEach(territory => {
        territory.neighbors.forEach(neighborId => {
            const connectionKey = [territory.id, neighborId].sort().join('-');
            if (!drawnConnections.has(connectionKey)) {
                drawnConnections.add(connectionKey);
                const neighbor = gameState.territories[neighborId];
                if (neighbor) {
                    const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
                    line.setAttribute('x1', territory.x);
                    line.setAttribute('y1', territory.y);
                    line.setAttribute('x2', neighbor.x);
                    line.setAttribute('y2', neighbor.y);
                    line.setAttribute('stroke', 'rgba(255,255,255,0.1)');
                    line.setAttribute('stroke-width', '1');
                    svg.appendChild(line);
                }
            }
        });
    });

    // Draw territories
    Object.values(gameState.territories).forEach(territory => {
        const group = document.createElementNS('http://www.w3.org/2000/svg', 'g');
        group.setAttribute('id', `territory-${territory.id}`);
        group.setAttribute('class', 'territory-group');

        // Territory circle
        const circle = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
        circle.setAttribute('cx', territory.x);
        circle.setAttribute('cy', territory.y);
        circle.setAttribute('r', 25);
        circle.setAttribute('class', 'territory');
        circle.setAttribute('data-id', territory.id);

        // Set color based on owner
        const color = territory.owner !== null ? FACTION_COLORS[territory.owner] : '#606080';
        circle.setAttribute('fill', color);

        // Click handler
        circle.addEventListener('click', () => handleTerritoryClick(territory.id));

        group.appendChild(circle);

        // Territory name
        const nameText = document.createElementNS('http://www.w3.org/2000/svg', 'text');
        nameText.setAttribute('x', territory.x);
        nameText.setAttribute('y', territory.y - 32);
        nameText.setAttribute('class', 'territory-label');
        nameText.textContent = territory.name;
        group.appendChild(nameText);

        // Points display
        const pointsText = document.createElementNS('http://www.w3.org/2000/svg', 'text');
        pointsText.setAttribute('x', territory.x);
        pointsText.setAttribute('y', territory.y + 5);
        pointsText.setAttribute('class', 'territory-points');
        pointsText.setAttribute('id', `points-${territory.id}`);
        pointsText.textContent = territory.points;
        group.appendChild(pointsText);

        svg.appendChild(group);
    });
}

// ===== Update Territory Visual =====
function updateTerritoryVisual(territoryId) {
    const territory = gameState.territories[territoryId];
    const circle = document.querySelector(`[data-id="${territoryId}"]`);
    const pointsText = document.getElementById(`points-${territoryId}`);

    if (circle) {
        const color = territory.owner !== null ? FACTION_COLORS[territory.owner] : '#606080';
        circle.setAttribute('fill', color);
    }

    if (pointsText) {
        pointsText.textContent = Math.floor(territory.points);
    }
}

// ===== Handle Territory Click =====
function handleTerritoryClick(territoryId) {
    if (gameState.gameOver || gameState.isPaused) return;

    const territory = gameState.territories[territoryId];

    // If no selection, select player's territory
    if (!gameState.selectedTerritory) {
        if (territory.owner === gameState.playerFactionId) {
            selectTerritory(territoryId);
        }
        return;
    }

    // If clicking same territory, deselect
    if (gameState.selectedTerritory === territoryId) {
        deselectTerritory();
        return;
    }

    const source = gameState.territories[gameState.selectedTerritory];

    // Check if target is a neighbor
    if (!source.neighbors.includes(territoryId)) {
        // Not a neighbor - switch selection if it's our territory
        if (territory.owner === gameState.playerFactionId) {
            selectTerritory(territoryId);
        }
        return;
    }

    // If clicking friendly neighbor territory, reinforce it
    if (territory.owner === gameState.playerFactionId) {
        // Send 50% of troops as reinforcement
        const reinforceAmount = Math.floor(source.points * 0.5);
        if (reinforceAmount >= CONFIG.minAttackPoints) {
            performReinforce(gameState.selectedTerritory, territoryId, reinforceAmount);
            deselectTerritory();
        } else {
            // Not enough troops, just switch selection
            selectTerritory(territoryId);
        }
        return;
    }

    // If clicking enemy/neutral territory, auto attack with 70% troops
    const attackAmount = Math.floor(source.points * 0.7);
    if (attackAmount >= CONFIG.minAttackPoints) {
        performAttack(gameState.selectedTerritory, territoryId, attackAmount);
        deselectTerritory();
    }
}

// ===== Select Territory =====
function selectTerritory(territoryId) {
    deselectTerritory();

    gameState.selectedTerritory = territoryId;
    const territory = gameState.territories[territoryId];

    // Highlight selected
    const circle = document.querySelector(`[data-id="${territoryId}"]`);
    if (circle) circle.classList.add('selected');

    // Highlight neighbors (attackable enemies and friendly for reinforce)
    territory.neighbors.forEach(neighborId => {
        const neighbor = gameState.territories[neighborId];
        const neighborCircle = document.querySelector(`[data-id="${neighborId}"]`);
        if (neighborCircle) {
            if (neighbor.owner === gameState.playerFactionId) {
                neighborCircle.classList.add('friendly');  // Can reinforce
            } else {
                neighborCircle.classList.add('targetable');  // Can attack
            }
        }
    });

    // Update UI
    updateSelectionPanel();
}

// ===== Deselect Territory =====
function deselectTerritory() {
    // Remove all highlights
    document.querySelectorAll('.territory').forEach(el => {
        el.classList.remove('selected', 'targetable', 'friendly');
    });

    gameState.selectedTerritory = null;
    gameState.targetTerritory = null;

    document.getElementById('selection-panel').style.display = 'none';
    document.getElementById('attack-controls').style.display = 'none';
}

// ===== Set Target Territory =====
function setTarget(territoryId) {
    gameState.targetTerritory = territoryId;

    const source = gameState.territories[gameState.selectedTerritory];
    const target = gameState.territories[territoryId];

    // Update slider
    const slider = document.getElementById('attack-slider');
    slider.max = Math.floor(source.points);
    slider.min = CONFIG.minAttackPoints;
    slider.value = Math.min(Math.floor(source.points), Math.floor(target.points) + 10);

    document.getElementById('attack-amount').textContent = slider.value;
    document.getElementById('attack-controls').style.display = 'block';

    updateSelectionPanel();
}

// ===== Update Selection Panel =====
function updateSelectionPanel() {
    const panel = document.getElementById('selection-panel');
    const info = document.getElementById('selected-info');

    if (!gameState.selectedTerritory) {
        panel.style.display = 'none';
        return;
    }

    panel.style.display = 'block';
    const source = gameState.territories[gameState.selectedTerritory];

    let html = `
        <div class="territory-name">${source.name}</div>
        <div class="territory-details">
            兵力: ${Math.floor(source.points)} / ${source.maxPoints}<br>
            区域: ${source.region}
        </div>
    `;

    if (gameState.targetTerritory) {
        const target = gameState.territories[gameState.targetTerritory];
        const targetFaction = target.owner !== null ? gameState.factions[target.owner] : null;
        html += `
            <div style="margin-top: 10px; padding-top: 10px; border-top: 1px solid #2a2a4a;">
                <div style="color: #f44336;">⚔️ 目标: ${target.name}</div>
                <div class="territory-details">
                    敌方兵力: ${Math.floor(target.points)}<br>
                    势力: ${targetFaction ? targetFaction.name : '中立'}
                </div>
            </div>
        `;
    }

    info.innerHTML = html;
}

// ===== Execute Attack =====
function executeAttack() {
    if (!gameState.selectedTerritory || !gameState.targetTerritory) return;

    const attackPoints = parseInt(document.getElementById('attack-slider').value);
    const source = gameState.territories[gameState.selectedTerritory];
    const target = gameState.territories[gameState.targetTerritory];

    if (attackPoints < CONFIG.minAttackPoints || attackPoints > source.points) return;

    performAttack(gameState.selectedTerritory, gameState.targetTerritory, attackPoints);
    deselectTerritory();
}

// ===== Perform Attack (shared by player and AI) =====
function performAttack(sourceId, targetId, attackPoints) {
    const source = gameState.territories[sourceId];
    const target = gameState.territories[targetId];
    const attacker = gameState.factions[source.owner];
    const defender = target.owner !== null ? gameState.factions[target.owner] : null;

    // Remove points from source
    source.points -= attackPoints;

    // Show attack animation
    showAttackAnimation(sourceId, targetId);

    // Resolve combat after animation
    setTimeout(() => {
        const previousOwner = target.owner;
        const defenderPoints = target.points;

        // Points cancel out - equal or greater attacker points wins
        if (attackPoints >= defenderPoints) {
            // Attacker wins (including tie - attacker advantage)
            target.owner = source.owner;
            target.points = Math.max(1, attackPoints - defenderPoints); // At least 1 point remains

            if (attackPoints === defenderPoints) {
                addLog(`${attacker.name} 险胜攻占了 ${target.name}！`, 'conquest');
            } else {
                addLog(`${attacker.name} 攻占了 ${target.name}！`, 'conquest');
            }

            // Check elimination
            if (previousOwner !== null) {
                checkElimination(previousOwner);
            }
        } else {
            // Defender holds
            target.points = defenderPoints - attackPoints;
            addLog(`${attacker.name} 进攻 ${target.name} 失败`, 'attack');
        }

        updateTerritoryVisual(sourceId);
        updateTerritoryVisual(targetId);
        updateUI();
        checkVictory();
    }, 500);
}

// ===== Perform Reinforcement =====
function performReinforce(sourceId, targetId, amount) {
    const source = gameState.territories[sourceId];
    const target = gameState.territories[targetId];
    const faction = gameState.factions[source.owner];

    // Transfer troops
    source.points -= amount;
    target.points = Math.min(target.points + amount, target.maxPoints);

    // Show reinforcement animation
    showReinforceAnimation(sourceId, targetId);

    addLog(`${faction.name} 向 ${target.name} 增援了 ${amount} 兵力`, 'reinforce');

    updateTerritoryVisual(sourceId);
    updateTerritoryVisual(targetId);
    updateUI();
}

// ===== Show Reinforcement Animation =====
function showReinforceAnimation(sourceId, targetId) {
    const svg = document.getElementById('map-svg');
    const source = gameState.territories[sourceId];
    const target = gameState.territories[targetId];

    // Create reinforce line (green/blue color)
    const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
    line.setAttribute('x1', source.x);
    line.setAttribute('y1', source.y);
    line.setAttribute('x2', target.x);
    line.setAttribute('y2', target.y);
    line.setAttribute('class', 'reinforce-line');
    line.setAttribute('stroke', '#4aff7a');
    line.setAttribute('stroke-width', '3');
    line.setAttribute('stroke-dasharray', '10,5');
    svg.appendChild(line);

    // Create arrival effect
    setTimeout(() => {
        const arrival = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
        arrival.setAttribute('cx', target.x);
        arrival.setAttribute('cy', target.y);
        arrival.setAttribute('r', '5');
        arrival.setAttribute('fill', '#4aff7a');
        arrival.setAttribute('class', 'reinforce-arrival');
        svg.appendChild(arrival);

        setTimeout(() => {
            arrival.remove();
        }, 400);
    }, 300);

    setTimeout(() => {
        line.remove();
    }, 500);
}

// ===== Show Attack Animation =====
function showAttackAnimation(sourceId, targetId) {
    const svg = document.getElementById('map-svg');
    const source = gameState.territories[sourceId];
    const target = gameState.territories[targetId];

    // Create attack line
    const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
    line.setAttribute('x1', source.x);
    line.setAttribute('y1', source.y);
    line.setAttribute('x2', target.x);
    line.setAttribute('y2', target.y);
    line.setAttribute('class', 'attack-line');
    line.setAttribute('stroke-dasharray', '100');
    svg.appendChild(line);

    // Create explosion
    setTimeout(() => {
        const explosion = document.createElementNS('http://www.w3.org/2000/svg', 'circle');
        explosion.setAttribute('cx', target.x);
        explosion.setAttribute('cy', target.y);
        explosion.setAttribute('r', '5');
        explosion.setAttribute('fill', '#ff4a4a');
        explosion.setAttribute('class', 'attack-explosion');
        svg.appendChild(explosion);

        setTimeout(() => {
            explosion.remove();
        }, 500);
    }, 400);

    setTimeout(() => {
        line.remove();
    }, 600);
}

// ===== Check Elimination =====
function checkElimination(factionId) {
    const territories = Object.values(gameState.territories).filter(t => t.owner === factionId);

    if (territories.length === 0) {
        const faction = gameState.factions[factionId];
        faction.isEliminated = true;
        addLog(`${faction.name} 已被消灭！`, 'elimination');
    }
}

// ===== Check Victory =====
function checkVictory() {
    const activeFactions = gameState.factions.filter(f => !f.isEliminated);

    if (activeFactions.length === 1) {
        gameState.gameOver = true;
        const winner = activeFactions[0];

        document.getElementById('victory-message').textContent =
            winner.isPlayer ? '恭喜！你征服了整个世界！' : `${winner.name} 获得了胜利...`;
        document.getElementById('victory-modal').style.display = 'flex';
    }
}

// ===== Game Loop (auto generate points) =====
function startGameLoop() {
    gameLoopInterval = setInterval(() => {
        if (gameState.isPaused || gameState.gameOver) return;

        Object.values(gameState.territories).forEach(territory => {
            if (territory.owner !== null && territory.points < territory.maxPoints) {
                // Use territory's individual growth rate
                territory.points += territory.growthRate * CONFIG.gameSpeed;
                territory.points = Math.min(territory.points, territory.maxPoints);
                updateTerritoryVisual(territory.id);
            }
        });

        gameState.turn++;
        document.getElementById('turn-info').textContent = `回合: ${gameState.turn}`;
    }, 1000);
}

// ===== AI System =====
function startAI() {
    gameState.factions.forEach(faction => {
        if (!faction.isPlayer) {
            const interval = setInterval(() => {
                if (gameState.isPaused || gameState.gameOver || faction.isEliminated) return;
                aiTurn(faction.id);
            }, CONFIG.aiDecisionInterval + Math.random() * 1000);
            aiIntervals.push(interval);
        }
    });
}

function aiTurn(factionId) {
    const myTerritories = Object.values(gameState.territories).filter(t => t.owner === factionId);
    if (myTerritories.length === 0) return;

    // Find best attack opportunity
    let bestScore = -Infinity;
    let bestAttack = null;

    myTerritories.forEach(source => {
        if (source.points < CONFIG.minAttackPoints + 10) return; // Keep some defense

        source.neighbors.forEach(neighborId => {
            const target = gameState.territories[neighborId];
            if (target.owner === factionId) return; // Don't attack own territory

            const availablePoints = source.points * 0.7; // Keep 30% for defense
            if (availablePoints < CONFIG.minAttackPoints) return;

            // Score the attack
            let score = 0;
            score += (availablePoints - target.points) * 2; // Prefer winnable fights
            if (target.owner === null) score += 20; // Prefer neutral
            if (availablePoints > target.points) score += 50; // Can win
            score += Math.random() * 20; // Some randomness

            if (score > bestScore) {
                bestScore = score;
                bestAttack = {
                    source: source.id,
                    target: target.id,
                    points: Math.min(Math.floor(availablePoints), Math.floor(target.points) + 15)
                };
            }
        });
    });

    // Execute if good opportunity
    if (bestAttack && bestScore > 30 && Math.random() < CONFIG.aiAggressiveness) {
        const source = gameState.territories[bestAttack.source];
        const target = gameState.territories[bestAttack.target];
        const faction = gameState.factions[factionId];

        addLog(`${faction.name} 进攻 ${target.name}`, 'attack');
        performAttack(bestAttack.source, bestAttack.target, bestAttack.points);
    }
}

// ===== Update UI =====
function updateUI() {
    const factionStatus = document.getElementById('faction-status');

    let html = '';
    gameState.factions.forEach(faction => {
        const territories = Object.values(gameState.territories).filter(t => t.owner === faction.id);
        const totalPoints = territories.reduce((sum, t) => sum + Math.floor(t.points), 0);

        html += `
            <div class="faction-item ${faction.isEliminated ? 'eliminated' : ''} ${faction.isPlayer ? 'player' : ''}">
                <div class="faction-color" style="background: ${faction.color}"></div>
                <span class="faction-name">${faction.name} ${faction.isPlayer ? '(你)' : ''}</span>
                <span class="faction-territories">${territories.length}块 / ${totalPoints}兵</span>
            </div>
        `;
    });

    factionStatus.innerHTML = html;
    document.getElementById('faction-count').textContent = `势力: ${gameState.factions.filter(f => !f.isEliminated).length}`;
}

// ===== Add Log Entry =====
function addLog(message, type = 'info') {
    const log = document.getElementById('game-log');
    const entry = document.createElement('div');
    entry.className = `log-entry ${type}`;
    entry.textContent = `[${gameState.turn}] ${message}`;
    log.insertBefore(entry, log.firstChild);

    // Keep only last 50 entries
    while (log.children.length > 50) {
        log.removeChild(log.lastChild);
    }
}

// ===== Event Listeners =====
document.addEventListener('DOMContentLoaded', () => {
    // Initialize game
    initGame(4);

    // Attack button
    document.getElementById('attack-btn').addEventListener('click', executeAttack);

    // Cancel button
    document.getElementById('cancel-btn').addEventListener('click', deselectTerritory);

    // Attack slider
    document.getElementById('attack-slider').addEventListener('input', (e) => {
        document.getElementById('attack-amount').textContent = e.target.value;
    });

    // Pause button
    document.getElementById('pause-btn').addEventListener('click', function () {
        gameState.isPaused = !gameState.isPaused;
        this.textContent = gameState.isPaused ? '▶️ 继续' : '⏸️ 暂停';
        this.classList.toggle('active', gameState.isPaused);
    });

    // Speed button
    document.getElementById('speed-btn').addEventListener('click', function () {
        CONFIG.gameSpeed = CONFIG.gameSpeed === 1 ? 3 : 1;
        this.textContent = CONFIG.gameSpeed === 1 ? '⏩ 加速' : '⏩ 正常';
        this.classList.toggle('active', CONFIG.gameSpeed > 1);
    });

    // Restart button
    document.getElementById('restart-btn').addEventListener('click', () => {
        const factionCount = parseInt(document.getElementById('faction-select').value);
        document.getElementById('victory-modal').style.display = 'none';
        initGame(factionCount);
    });

    // Play again button
    document.getElementById('play-again-btn').addEventListener('click', () => {
        const factionCount = parseInt(document.getElementById('faction-select').value);
        document.getElementById('victory-modal').style.display = 'none';
        initGame(factionCount);
    });

    // Faction count change
    document.getElementById('faction-select').addEventListener('change', (e) => {
        initGame(parseInt(e.target.value));
    });
});
