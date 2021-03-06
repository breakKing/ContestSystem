<template lang="ru">
    <h1 id="tableLabel">Contests</h1>

    <table class='table table-striped' aria-labelledby="tableLabel">
        <thead>
            <tr>
                <th>ID</th>
                <th>Название</th>
                <th>Описание</th>
                <th>Открытый</th>
                <th>Постоянный</th>
                <th>Начало</th>
                <th>Конец</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="contest of contests" v-bind:key="contest">
                <td>{{ contest.id }}</td>
                <td>{{ contest.name }}</td>
                <td>{{ contest.description }}</td>
                <td v-if="contest.isPublic">Да</td>
                <td v-else>Нет</td>
                <td v-if="contest.isForever">Да</td>
                <td v-else>Нет</td>
                <td>{{ contest.startDateTime }}</td>
                <td>{{ contest.endDateTime }}</td>
            </tr>
        </tbody>
    </table>

    <h1 id="tableLabel">Problems</h1>

    <table class='table table-striped' aria-labelledby="tableLabel">
        <thead>
            <tr>
                <th>ID</th>
                <th>Название</th>
                <th>Описание</th>
                <th>Входные данные</th>
                <th>Выходные данные</th>
                <th>Ограничения по времени</th>
                <th>Ограничения по памяти</th>
                <th>Тип</th>
                <th>Общедоступная</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="problem of problems" v-bind:key="problem">
                <td>{{ problem.id }}</td>
                <td>{{ problem.name }}</td>
                <td>{{ problem.description }}</td>
                <td>{{ problem.inputBlock }}</td>
                <td>{{ problem.outputBlock }}</td>
                <td>{{ problem.timeLimit }}</td>
                <td>{{ problem.memoryLimit }}</td>
                <td v-if="problem.type === 0">Полное решение</td>
                <td v-else>Частичное решение</td>
                <td v-if="problem.isPublic">Да</td>
                <td v-else>Нет</td>
            </tr>
        </tbody>
    </table>

    <h1 id="tableLabel">Examples</h1>

    <table class='table table-striped' aria-labelledby="tableLabel">
        <thead>
            <tr>
                <th>ID</th>
                <th>Номер</th>
                <th>Ввод</th>
                <th>Вывод</th>
                <th>ID задачи</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="example of examples" v-bind:key="example">
                <td>{{ example.id }}</td>
                <td>{{ example.number }}</td>
                <td>{{ example.inputText }}</td>
                <td>{{ example.outputText }}</td>
                <td>{{ example.problem.id }}</td>
            </tr>
        </tbody>
    </table>

    <h1 id="tableLabel">ContestsProblems</h1>

    <table class='table table-striped' aria-labelledby="tableLabel">
        <thead>
            <tr>
                <th>ID</th>
                <th>ID задачи</th>
                <th>ID соревнования</th>
                <th>Буква задачи</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="cp of contests_problems" v-bind:key="alias">
                <td>{{ cp.id }}</td>
                <td>{{ cp.problemId }}</td>
                <td>{{ cp.contestId }}</td>
                <td>{{ cp.alias }}</td>
            </tr>
        </tbody>
    </table>
</template>


<script>
    import axios from 'axios'
    export default {
        name: "TestDb",
        data() {
            return {
                contests: [],
                examples: [],
                problems: [],
                contests_problems: []
            }
        },
        methods: {
            async getContests() {
                await axios.get('/api/TestDb/contests')
                    .then((response) => {
                        this.contests = response.data;
                    })
                    .catch(function (error) {
                        alert(error);
                    });
            },
            async getProblems() {
                await axios.get('/api/TestDb/problems')
                    .then((response) => {
                        this.problems = response.data;
                    })
                    .catch(function (error) {
                        alert(error);
                    });
            },
            async getExamples() {
                await axios.get('/api/TestDb/examples')
                    .then((response) => {
                        this.examples = response.data;
                    })
                    .catch(function (error) {
                        alert(error);
                    });
            },
            async getContestsProblems() {
                await axios.get('/api/TestDb/contests_problems')
                    .then((response) => {
                        this.contests_problems = response.data;
                    })
                    .catch(function (error) {
                        alert(error);
                    });
            }
        },
        async mounted() {
            await this.getContests();
            await this.getProblems();
            await this.getExamples();
            await this.getContestsProblems();
        }
    }
</script>