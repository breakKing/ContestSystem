<template>
  <div class="side-bar-menu-light main-block-height d-flex flex-column justify-content-start me-3"
       style="padding: 10px">

    <h4 class="fw-bold fs-4 m-2 mb-5 ">ContestSystem Workspace</h4>
    <hr>
    <ul class="list-unstyled underlined_li ps-0 ">
      <li class="mb-1">
        <button class="btn btn-custom rounded align-items-center ">
          Мой профиль
        </button>
      </li>
      <!--      <li class="mb-1">-->
      <!--        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed " data-bs-toggle="collapse"-->
      <!--                data-bs-target="#courses-collapse" aria-expanded="false">-->
      <!--          Курсы-->
      <!--        </button>-->
      <!--        <div class="collapse" id="courses-collapse">-->
      <!--          <ul class="btn-toggle-nav underlined_li list-unstyled fw-normal pb-1 small">-->
      <!--            <li>-->
      <!--              <a href="#" class="no_style_link rounded">Ожидающие модерации</a>-->
      <!--            </li>-->
      <!--            <li>-->
      <!--              <a href="#" class="no_style_link rounded">Одобренные</a>-->
      <!--            </li>-->
      <!--            <li>-->
      <!--              <a href="#" class="no_style_link rounded">Отклонённые</a>-->
      <!--            </li>-->
      <!--            <li>-->
      <!--              <a href="#" class="no_style_link rounded">Все</a>-->
      <!--            </li>-->
      <!--          </ul>-->
      <!--        </div>-->
      <!--      </li>-->
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed "
                data-bs-toggle="collapse"
                data-bs-target="#posts-collapse" aria-expanded="false">
          Посты
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyPendingPostsPage','WorkSpaceMyApprovedPostsPage','WorkSpaceMyRejectedPostsPage','WorkSpaceMyPostsPage'].includes($route.name)}"
             id="posts-collapse">
          <ul class="btn-toggle-nav underlined_li list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyPendingPostsPage'}">Ожидающие модерации
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyApprovedPostsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyRejectedPostsPage'}">Отклонённые
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyPostsPage'}">Все
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed "
                data-bs-toggle="collapse"
                data-bs-target="#contests-collapse" aria-expanded="false">
          Соревнования
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyPendingContestsPage','WorkSpaceMyApprovedContestsPage','WorkSpaceMyRejectedContestsPage','WorkSpaceMyContestsPage'].includes($route.name)}"
             id="contests-collapse">
          <ul class="btn-toggle-nav underlined_li list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyPendingContestsPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyApprovedContestsPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyRejectedContestsPage'}">Отклонённые
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyContestsPage'}">Все
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed "
                data-bs-toggle="collapse"
                data-bs-target="#tasks-collapse" aria-expanded="false">
          Задачи
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyPendingTasksPage','WorkSpaceMyApprovedTasksPage','WorkSpaceMyRejectedTasksPage','WorkSpaceAllTasksPage'].includes($route.name)}"
             id="tasks-collapse">
          <ul class="btn-toggle-nav underlined_li list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyPendingTasksPage'}">Ожидающие модерации
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyApprovedTasksPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyRejectedTasksPage'}">Отклонённые
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceAllTasksPage'}">Каталог
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed "
                data-bs-toggle="collapse"
                data-bs-target="#checkers-collapse" aria-expanded="false">
          Механизмы проверки
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyPendingCheckersPage','WorkSpaceMyApprovedCheckersPage','WorkSpaceMyRejectedCheckersPage','WorkSpaceAllCheckersPage'].includes($route.name)}"
             id="checkers-collapse">
          <ul class="btn-toggle-nav underlined_li list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyPendingCheckersPage'}">Ожидающие
                модерации
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyApprovedCheckersPage'}">Одобренные
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyRejectedCheckersPage'}">Отклонённые
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceAllCheckersPage'}">Каталог
              </router-link>
            </li>
          </ul>
        </div>
      </li>
      <li class="mb-1">
        <button class="btn btn-custom btn-toggle align-items-center rounded collapsed"
                data-bs-toggle="collapse"
                data-bs-target="#rule-sets-collapse" aria-expanded="false">
          Наборы правил
        </button>
        <div class="collapse"
             :class="{show: ['WorkSpaceMyRuleSetsPage','WorkSpaceAllRuleSetsPage'].includes($route.name)}"
             id="rule-sets-collapse">
          <ul class="btn-toggle-nav underlined_li list-unstyled fw-normal pb-1 small">
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceMyRuleSetsPage'}">Мои
              </router-link>
            </li>
            <li>
              <router-link class="no_style_link rounded" :to="{name: 'WorkSpaceAllRuleSetsPage'}">Каталог
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
.side-bar-menu-light {
  .btn-custom.rounded {
    font-size: 1.2rem;
  }

  .no_style_link.rounded {
    font-size: 1.1rem;
  }
  
}
</style>