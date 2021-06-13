<template>
  <div class="flex-shrink-0 bg-primary d-flex flex-column justify-content-start"
       style=" height: 100%; padding: 10px">

    <h4 class="fw-bold m-2 mb-5 text-light">ContestSystem Workspace</h4>

    <ul class="list-unstyled ps-0 ">
      <li class="mb-1">
        <button class="btn align-items-center text-light">
          Мой профиль
        </button>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#courses-collapse" aria-expanded="false">
          Курсы
        </button>
        <div class="collapse" id="courses-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <a href="#" class="link-light rounded">Ожидающие модерации</a>
            </li>
            <li>
              <a href="#" class="link-light rounded">Одобренные</a>
            </li>
            <li>
              <a href="#" class="link-light rounded">Отклонённые</a>
            </li>
            <li>
              <a href="#" class="link-light rounded">Все</a>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#posts-collapse" aria-expanded="false">
          Посты
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyPendingPostsPage','WorkSpaceMyApprovedPostsPage','WorkSpaceMyRejectedPostsPage','WorkSpaceMyPostsPage'].includes($route.name)}"
             id="posts-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyPendingPostsPage'}">Ожидающие модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyApprovedPostsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyRejectedPostsPage'}">Отклонённые
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyPostsPage'}">Все
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#contests-collapse" aria-expanded="false">
          Соревнования
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyPendingContestsPage','WorkSpaceMyApprovedContestsPage','WorkSpaceMyRejectedContestsPage','WorkSpaceMyContestsPage'].includes($route.name)}"
             id="contests-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyPendingContestsPage'}">Ожидающие модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyApprovedContestsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyRejectedContestsPage'}">Отклонённые
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyContestsPage'}">Все
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#tasks-collapse" aria-expanded="false">
          Задачи
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyPendingTasksPage','WorkSpaceMyApprovedTasksPage','WorkSpaceMyRejectedTasksPage','WorkSpaceAllTasksPage'].includes($route.name)}"
             id="tasks-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyPendingTasksPage'}">Ожидающие модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyApprovedTasksPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyRejectedTasksPage'}">Отклонённые
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceAllTasksPage'}">Каталог
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#checkers-collapse" aria-expanded="false">
          Механизмы проверки
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyPendingCheckersPage','WorkSpaceMyApprovedCheckersPage','WorkSpaceMyRejectedCheckersPage','WorkSpaceAllCheckersPage'].includes($route.name)}"
             id="checkers-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyPendingCheckersPage'}">Ожидающие модерации
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyApprovedCheckersPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyRejectedCheckersPage'}">Отклонённые
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceAllCheckersPage'}">Каталог
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li style="margin-bottom: 135px">
        <button class="btn btn-toggle align-items-center rounded collapsed text-light" data-bs-toggle="collapse"
                data-bs-target="#rule-sets-collapse" aria-expanded="false">
          Наборы правил
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyRuleSetsPage','WorkSpaceAllRuleSetsPage'].includes($route.name)}"
             id="rule-sets-collapse">
          <ul class="btn-toggle-nav list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceMyRuleSetsPage'}">Мои
              </router-link>
            </li>
            <li>
              <router-link class="link-light rounded" :to="{name: 'WorkSpaceAllRuleSetsPage'}">Каталог
              </router-link>
            </li>
          </ul>
        </div>
      </li>
    </ul>
  </div>
</template>

<script>
export default {
  name: "WorkSpaceSideBarComponent"
}
</script>

<style lang="scss" scoped>
h4 {
  font-family: cursive;
}

.btn {
  display: inline-flex;
  align-items: center;
  padding: .25rem .5rem;
  font-weight: 600;
  font-size: 22px;
  color: rgba(0, 0, 0, .65);
  background-color: transparent;
  border: 0;
}

.btn:hover,
.btn:focus,
li:hover,
li:focus {
  color: rgba(0, 0, 0, .85);
  background-color: rgba(210, 244, 234, 0.188);
}

.btn-toggle::before {
  width: 1.25em;
  line-height: 0;
  content: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 16 16'%3e%3cpath fill='none' stroke='rgba%280,0,0,.5%29' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M5 14l6-6-6-6'/%3e%3c/svg%3e");
  transition: transform .35s ease;
  transform-origin: .5em 50%;
}

.btn-toggle[aria-expanded="true"] {
  color: rgba(0, 0, 0, .85);
}

.btn-toggle[aria-expanded="true"]::before {
  transform: rotate(90deg);
}

.btn-toggle-nav a {
  display: inline-flex;
  padding: .1875rem .5rem;
  margin-top: .125rem;
  margin-left: 1.25rem;
  text-decoration: none;
}

.router-link-active {
  background-color: #0A5BF0;
}

.link-light {
  font-size: 18px;
}
</style>