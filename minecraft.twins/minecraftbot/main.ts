// https://github.com/PrismarineJS/mineflayer

const mineflayer = require('mineflayer')
const { mineflayer: mineflayerViewer } = require('prismarine-viewer')
const { pathfinder, Movements } = require('mineflayer-pathfinder')
const { GoalNear, GoalBlock, GoalXZ, GoalY, GoalInvert, GoalFollow } = require('mineflayer-pathfinder').goals

const bot = mineflayer.createBot({
    host: process.env.MINECRAFT_HOST, // optional ('51.105.170.25')
    port: process.env.MINECRAFT_PORT, // optional (25565)
    username: process.env.MINECRAFT_BOT_NAME
    //   password: 'minecraft',          // online-mode=true servers
    //   version: false                 // false corresponds to auto version detection (that's the default), put for example "1.8.8" if you need a specific version
})

bot.loadPlugin(pathfinder)

bot.once('spawn', () => {
    // Once we've spawn, it is safe to access mcData because we know the version
    const mcData = require('minecraft-data')(bot.version)

    // We create different movement generators for different type of activity
    const defaultMove = new Movements(bot, mcData)

    mineflayerViewer(bot, { port: 8181, firstPerson: true }) // port is the minecraft server port, if first person is false, you get a bird's-eye view

    bot.on('path_update', (r) => {
        const nodesPerTick = (r.visitedNodes * 50 / r.time).toFixed(2)
        console.log(`I can get there in ${r.path.length} moves. Computation took ${r.time.toFixed(2)} ms (${nodesPerTick} nodes/tick).`)
    })

    bot.on('goal_reached', (goal) => {
        bot.chat('Here I am !')
    })

    bot.on('chat', (username, message) => {
        if (username === bot.username) return

        const target = bot.players[username] ? bot.players[username].entity : null
        if (message === 'come') {
            if (!target) {
                bot.chat('I don\'t see you !')
                return
            }
            const p = target.position

            bot.pathfinder.setMovements(defaultMove)
            bot.pathfinder.setGoal(new GoalNear(p.x, p.y, p.z, 1))
        } else if (message.startsWith('goto')) {
            const cmd = message.split(' ')

            if (cmd.length === 4) { // goto x y z
                const x = parseInt(cmd[1], 10)
                const y = parseInt(cmd[2], 10)
                const z = parseInt(cmd[3], 10)

                bot.pathfinder.setMovements(defaultMove)
                bot.pathfinder.setGoal(new GoalBlock(x, y, z))
            } else if (cmd.length === 3) { // goto x z
                const x = parseInt(cmd[1], 10)
                const z = parseInt(cmd[2], 10)

                bot.pathfinder.setMovements(defaultMove)
                bot.pathfinder.setGoal(new GoalXZ(x, z))
            } else if (cmd.length === 2) { // goto y
                const y = parseInt(cmd[1], 10)

                bot.pathfinder.setMovements(defaultMove)
                bot.pathfinder.setGoal(new GoalY(y))
            }
        } else if (message === 'follow') {
            bot.pathfinder.setMovements(defaultMove)
            bot.pathfinder.setGoal(new GoalFollow(target, 3), true)
            // follow is a dynamic goal: setGoal(goal, dynamic=true)
            // when reached, the goal will stay active and will not
            // emit an event
        } else if (message === 'avoid') {
            bot.pathfinder.setMovements(defaultMove)
            bot.pathfinder.setGoal(new GoalInvert(new GoalFollow(target, 5)), true)
        } else if (message === 'stop') {
            bot.pathfinder.setGoal(null)
        }
    })
})

// Log errors and kick reasons:
bot.on('kicked', (reason, loggedIn) => console.log(reason, loggedIn))
bot.on('error', err => console.log(err))

var express = require('express');
var bodyParser = require('body-parser');

var app = express();

// parse application/cloudevents+json
app.use(bodyParser.json({ type: 'application/cloudevents+json' }));

const port = 8080

app.get('/dapr/subscribe', (req, res) => {
    res.json([
        {
            pubsubname: "messagebus",
            topic: "temperature",
            route: "temperature"
        }
    ]);
})

app.post('/temperature', (req, res) => {
    try {
        let message = req.body.data;
        console.log(`Average: ${message}`);

        bot.chat(`Average: ${message}`);
        res.sendStatus(200);
    }
    catch (e) {
        console.log(e);
        res.sendStatus(500);
    }
});

app.listen(port, () => console.log(`minecraft bot listening on port ${port}`))