export default function (contest_id) {
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
            to: {name: 'ContestMonitorPage', params: {contest_id}},
            breadcrumbName: 'Монитор',
        },
    ]
}
