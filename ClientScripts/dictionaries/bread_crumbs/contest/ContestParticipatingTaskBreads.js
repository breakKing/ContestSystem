export default function (contest_id, task_id) {
    return [
        {
            to: {name: 'UserStarterPage',},
            breadcrumbName: 'Домой',
        },
        {
            breadcrumbName: 'Соревнования',
            children: [
                {
                    to: {name: 'AvailableContestsPage',},
                    breadcrumbName: 'Доступные',
                },
                {
                    to: {name: 'ParticipatingContestsPage',},
                    breadcrumbName: 'Принимаю участие',
                },
                {
                    to: {name: 'CurrentlyRunningContestsComponentPage',},
                    breadcrumbName: 'Проходящие сейчас',
                },
            ]
        },
        {
            to: {name: 'ContestPage', params: {contest_id}},
            breadcrumbName: 'Соревнование',
        },
        {
            to: {name: 'ContestParticipatingPage', params: {contest_id, task_id}},
            breadcrumbName: 'Задача',
        },
    ]
}
