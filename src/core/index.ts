import { Client } from 'discord.js'
import { Command } from './models/commands'
import glob from 'glob'
import { commandHandler } from './events/commandHandler'
import { IBot } from './models/bot'
import { exit } from 'process'
import { ICommandInvoke, ICommandsInvoke } from './models/commandInvoke'
import { rateLimitHandler } from './events/rateLimitHandler'

class KurosawaDia implements IBot {
    readonly client: Client
    private _token: string
    commands: ICommandsInvoke
    uniqueCommands: ICommandsInvoke

    constructor () {
        this.client = new Client()
        this._token = ''
        this.commands = {}
        this.uniqueCommands = {}
    }

    set token (value: string) {
        if (!value) {
            console.log('Token has not been defined')
            process.exit()
        }

        this._token = value
    }

    registerCommand (Class: any): void {
        const commandInvoke: ICommandInvoke = {
            module: Reflect.getMetadata('command:info', Class).module,
            ClassDefinition: Class,
            name: Reflect.getMetadata('command:name', Class),
            alias: Reflect.getMetadata('command:alias', Class)
        }

        if (!commandInvoke.name) {
            return
        }

        if (this.commands[commandInvoke.name]) {
            console.log('Comando ' + commandInvoke.name + ' already exists')
            exit()
        }

        this.commands[commandInvoke.name] = commandInvoke
        this.uniqueCommands[commandInvoke.name] = commandInvoke
        if (commandInvoke.alias) {
            for (const alias of commandInvoke.alias) {
                if (!this.commands[alias]) {
                    this.commands[alias] = commandInvoke
                } else {
                    console.log('Comando ' + alias + ' already exists')
                    exit()
                }
            }
        }
    }

    registerCommands (): void {
        this.commands = {}

        glob('./src/core/commands/**/*.ts', {
            absolute: true
        }, (error, files) => {
            if (error) {
                console.error(error)
                return
            }

            console.log('Loading commands')
            let i = 0
            for (const file of files) {
                const ClassDefinition = require(file).default

                if (!ClassDefinition) {
                    console.log('Commando is not using export default')
                    exit()
                }

                const command = new ClassDefinition()

                if (command instanceof Command) {
                    this.registerCommand(ClassDefinition)
                    i++
                }
            }
            console.log(i + ' commands load')
            console.table(this.uniqueCommands, [
                'module',
                'name',
                'ClassDefinition'
            ])
        })
    }

    start (): void {
        this.client.on('ready', () => {
            console.log('Bot start')
        })

        this.client.on('message', async message => {
            await commandHandler(message, this.commands, this)
        })

        this.client.on('rateLimit', async info => {
            await rateLimitHandler(info)
        })

        this.client.login(this._token)
    }
}

const kurosawaDia = new KurosawaDia()

export default kurosawaDia
